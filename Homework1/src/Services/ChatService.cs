using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Homework1.Database;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Services.Repositories;
using Qoden.Validation;

namespace Homework1.Services
{
    public interface IChatService
    {
        Task<ChatResponse> CreateChat(int userId, string name);
        Task AddUserToChat(int chatId, int userId, int adminId);
        Task DeleteUserFromChat(int chatId, int userId, int adminId);
        Task<List<int>> GetChatsId(int userId);
        Task<MessageResponse> SendMessage(int userId, SendMessageRequest request);
        Task<List<ChatResponse>> GetChats(int userId);
        Task<List<UserInfoResponse>> GetUsersFromChat(int chatId, int adminId);
    }

    public class ChatService : IChatService
    {
        private readonly IDbConnectionFactory _dbConnFactory;

        public ChatService(IDbConnectionFactory factory)
        {
            _dbConnFactory = factory;
        }

        public async Task<ChatResponse> CreateChat(int userId, string name)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                var user = await conn.GetUserById(userId);
                Check.Value(user).NotNull("User doesn't exist");

                var id = await conn.QueryAsync<int>($"INSERT INTO chats (name, admin_id) VALUES ('{name}', '{user.Id}'); " +
                                           $"SELECT lastval();");

                var chatAdmin = AutoMapper.Mapper.Map<User, UserInfoResponse>(user);
                return new ChatResponse()
                {
                    ChatId = id.First(),
                    Name = name,
                    Messages = null,
                    Admin = chatAdmin,
                    Users = new List<UserInfoResponse>() {chatAdmin}
                };
            }
        }

        public async Task AddUserToChat(int chatId, int userId, int adminId)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                var admin = await conn.QueryFirstOrDefaultAsync<int?>
                    ($"SELECT admin_id FROM chats WHERE id='{chatId}' and admin_id='{adminId}'");
                Check.Value(admin, "Access failed").NotNull("You don't have rules for add this user");

                var id = await conn.QueryFirstOrDefaultAsync<int?>("SELECT id FROM chats WHERE id=@Id", new {Id = chatId});
                Check.Value(id).NotNull("Chat doesn't exist");

                var userChat = await conn.QueryFirstOrDefaultAsync<UserChat>
                    ($"SELECT * FROM user_chats WHERE chat_id='{chatId}' and user_id='{userId}'");

                if (userChat != null)
                {
                    userChat.User = null;
                    userChat.Chat = null;
                }

                Check.Value(userChat).IsNull("User already in this chat");

                await conn.ExecuteAsync("INSERT INTO user_chats (chat_id, user_id, flag) VALUES (@ChatId, @UserId, @Flag)",
                    new UserChat()
                {
                    ChatId = id.Value,
                    UserId = userId,
                    Flag = true
                });
            }
        }

        public async Task DeleteUserFromChat(int chatId, int userId, int adminId)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                var admin = await conn.QueryFirstOrDefaultAsync<int?>
                    ($"SELECT admin_id FROM chats WHERE id='{chatId}' and admin_id='{adminId}'");
                Check.Value(admin, "Access failed").NotNull("You don't have rules for delete this user");

                var userChat = await conn.QueryFirstOrDefaultAsync<UserChat>
                    ($"SELECT * FROM user_chats WHERE chat_id='{chatId}' and user_id='{userId}'");
                Check.Value(userChat).NotNull();

                await conn.ExecuteAsync($"DELETE FROM user_chats WHERE user_id='{userId}' " +
                                        $"and chat_id='{chatId}'");
            }
        }

        public async Task<MessageResponse> SendMessage(int userId, SendMessageRequest request)
        {
            request.Validate(ImmediateValidator.Instance);

            using (var conn = _dbConnFactory.CreateConnection())
            {
                var userChat = await conn.QueryFirstOrDefaultAsync<UserChat>
                        ($"SELECT * FROM user_chats WHERE chat_id='{request.ChatId}' and user_id='{userId}'");
                Check.Value(userChat).NotNull();

                var message = new Message()
                {
                    UserId = userId,
                    ChatId = request.ChatId,
                    Text = request.Message,
                    InvitedAt = DateTime.Now
                };

                await conn.ExecuteAsync("INSERT INTO messages (user_id, chat_id, text, invited_at) " +
                                        "VALUES (@UserId, @ChatId, @Text, @InvitedAt)", message);

                return AutoMapper.Mapper.Map<Message, MessageResponse>(message);
            }
        }

        public async Task<List<int>> GetChatsId(int userId)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                return (await conn.QueryAsync<int>
                    ($"SELECT chat_id FROM user_chats WHERE user_id='{userId}' and flag='{true}'")).ToList();
            }
        }

        public async Task<List<ChatResponse>> GetChats(int userId)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                return (await conn.QueryAsync<Chat>
                    ($"SELECT * FROM chats WHERE user_id='{userId}' and flag='{true}'")).Select(u =>
                    AutoMapper.Mapper.Map<Chat, ChatResponse>(u)).ToList();
            }
        }

        public async Task<List<UserInfoResponse>> GetUsersFromChat(int chatId, int adminId)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                var admin = await conn.QueryFirstOrDefaultAsync<int?>
                    ($"SELECT admin_id FROM chats WHERE id='{chatId}' and admin_id='{adminId}'");
                Check.Value(admin, "Access failed").NotNull("You don't have rules for viewing this chat");

                return (await conn.QueryAsync<User>
                    ($"SELECT * FROM users WHERE user_chats.chat_id='{chatId}' and " +
                     $"user_chats.user_id=users.id"))
                    .Select(u => AutoMapper.Mapper.Map<User, UserInfoResponse>(u)).ToList();
            }
        }
    }
}