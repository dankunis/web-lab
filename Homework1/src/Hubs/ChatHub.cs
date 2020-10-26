using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Qoden.Util;
using Qoden.Validation;

namespace Homework1.Hubs
{
    [Authorize]
    public class ChatHub : BaseHub
    {
        private readonly IChatService _chatService;
        private static ConcurrentDictionary<int, List<Message>> _messages = new ConcurrentDictionary<int, List<Message>>();
        private static ConcurrentDictionary<int, HashSet<string>> _usersConnections = new ConcurrentDictionary<int, HashSet<string>>();
        private static ConcurrentDictionary<int, HashSet<string>> _chatsConnections = new ConcurrentDictionary<int, HashSet<string>>();

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var chats = await _chatService.GetChatsId(this.UserId);

            _usersConnections.GetOrAdd(this.UserId, new HashSet<string>()).Add(Context.ConnectionId);

            foreach (var chat in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chat.ToString());
                _chatsConnections.GetOrAdd(chat, new HashSet<string>()).Add(Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var chats = await _chatService.GetChatsId(this.UserId);

            _usersConnections[this.UserId].Remove(Context.ConnectionId);

            foreach (var chat in chats)
            {
                _chatsConnections[chat].Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageRequest request)
        {
            var message = new Message()
            {
                UserId = this.UserId,
                ChatId = request.ChatId,
                Text = request.Message,
                InvitedAt = DateTime.Now
            };
            var response = AutoMapper.Mapper.Map<Message, MessageResponse>(message);
            response.UserEmail = this.UserLogin;

            request.Validate(ImmediateValidator.Instance);
            Check.Value(_chatsConnections.ContainsKey(request.ChatId)).IsTrue();
            Check.Value(_chatsConnections[request.ChatId].Contains(Context.ConnectionId)).IsTrue();

            _messages.GetOrAdd(request.ChatId, new List<Message>()).Add(message);
            await Clients.Group(request.ChatId.ToString()).SendAsync("SendMessage", response);
        }

        public async Task CreateChat(string name)
        {
            var response = await _chatService.CreateChat(this.UserId, name);

            _messages[response.ChatId] = new List<Message>();
            _usersConnections.GetOrAdd(this.UserId, new HashSet<string>())
                .ForEach(async connId =>
                {
                    _chatsConnections.GetOrAdd(response.ChatId, new HashSet<string>()).Add(connId);
                    await Groups.AddToGroupAsync(connId, response.ChatId.ToString());
                });

            await Clients.Caller.SendAsync("CreateChat", response);
        }

        public async Task AddUserToChat(int chatId, int userId)
        {
            await _chatService.AddUserToChat(chatId, userId, this.UserId);

            _usersConnections.GetOrAdd(userId, new HashSet<string>()).ForEach(async connId =>
            {
                _chatsConnections[chatId].Add(connId);
                await Groups.AddToGroupAsync(connId, chatId.ToString());
            });

            await Clients.Group(chatId.ToString()).SendAsync("AddUserToChat", userId, chatId);
        }

        public async Task DeleteUserFromChat(int chatId, int userId)
        {
            await _chatService.DeleteUserFromChat(chatId, userId, this.UserId);

            _usersConnections.GetOrAdd(userId, new HashSet<string>()).ForEach(async connId =>
            {
                _chatsConnections[chatId].Remove(connId);
                await Groups.RemoveFromGroupAsync(connId, chatId.ToString());
            });

            await Clients.Group(chatId.ToString()).SendAsync("DeleteUserFromChat", userId, chatId);
        }

        public async Task<List<ChatResponse>> GetChats()
        {
            return await _chatService.GetChats(this.UserId);
        }

        public async Task<List<UserInfoResponse>> GetUsersFromChat(int chatId)
        {
            return await _chatService.GetUsersFromChat(chatId, this.UserId);
        }
    }
}