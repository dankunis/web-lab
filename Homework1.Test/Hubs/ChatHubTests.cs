using System;
using System.Threading.Tasks;
using FluentAssertions;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Test.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Qoden.Validation;
using Xunit;
using Xunit.Abstractions;

namespace Homework1.Test.Hubs
{
    [Collection(ApiCollectionFixture.Name)]
    public class ChatHubTests
    {
        public ApiFixture Api { get; set; }

        private ITestOutputHelper _output;
        private static readonly Uri _uri = new Uri("https://localhost:5001/ws/chat");

        public ChatHubTests(ApiFixture api, ITestOutputHelper output)
        {
            Api = api;
            _output = output;
        }

        [Fact]
        public async Task UserCanSendMessageInHisChat()
        {
            MessageResponse response = null;
            var numberOfCalls = 0;

            var connection = (await new HubConnectionBuilder()
                    .WithUrl(_uri, TestData.User.Email, TestData.User.Password))
                    .Build();
            var connection2 = (await new HubConnectionBuilder()
                    .WithUrl(_uri, TestData.Admin.Email, TestData.Admin.Password))
                    .Build();
            var connection3 = (await new HubConnectionBuilder()
                    .WithUrl(_uri, TestData.Manager.Email, TestData.Manager.Password))
                    .Build();

            await connection.StartAsync();
            await connection2.StartAsync();
            await connection3.StartAsync();

            connection.On<MessageResponse>("SendMessage", res =>
            {
                response = res;
                numberOfCalls++;
            });
            connection2.On<MessageResponse>("SendMessage", res => { numberOfCalls++; });
            connection3.On<MessageResponse>("SendMessage", res => { numberOfCalls++; });

            await connection.InvokeAsync("SendMessage", new SendMessageRequest()
            {
                ChatId = 1,
                Message = "Hello"
            });

            numberOfCalls.Should().Be(3);
            response.ChatId.Should().Be(1);
            response.Text.Should().BeEquivalentTo("Hello");
            response.UserEmail.Should().BeEquivalentTo("user@gmail.com");
        }

        [Fact]
        public async Task UserCannotSendMessageInNotHisChat()
        {
            HubException exception = null;
            var connection = (await new HubConnectionBuilder()
                    .WithUrl(_uri, TestData.User.Email, TestData.User.Password))
                    .Build();

            await connection.StartAsync();
            connection.On<MessageResponse>("SendMessage", res => { });

            try
            {
                await connection.InvokeAsync("SendMessage", new SendMessageRequest()
                {
                    ChatId = 7,
                    Message = "Hello"
                });
            }
            catch (HubException e)
            {
                exception = e;
            }

            exception.Should().NotBeNull();
            exception?.Message.Should().BeEquivalentTo("An unexpected error occurred invoking 'SendMessage' on the server.");
        }

        [Fact]
        public async Task UserCanCreateChat()
        {
            var user = AutoMapper.Mapper.Map<User, UserInfoResponse>(TestData.Admin);
            ChatResponse response = null;

            var connection = (await new HubConnectionBuilder()
                    .WithUrl(_uri, TestData.Admin.Email, TestData.Admin.Password))
                    .Build();

            await connection.StartAsync();
            connection.On<ChatResponse>("CreateChat", (res) => { response = res; });

            await connection.InvokeAsync("CreateChat", "Chat name");

            user.Should().BeEquivalentTo(response.Admin);
            response.Users.Count.Should().Be(1);
            response.Users[0].Should().BeEquivalentTo(user);
            response.Messages.Should().BeNull();
            response.Name.Should().BeEquivalentTo("Chat name");
        }

        [Fact]
        public async Task ChatAdminCanAddUsersInHisChat()
        {
            var chatId = 0;
            var userId = 0;
            var connection = (await new HubConnectionBuilder()
                    .WithUrl(_uri, TestData.Admin.Email, TestData.Admin.Password))
                .Build();

            await connection.StartAsync();
            connection.On<int, int>("AddUserToChat", (uId, cId) => {
                chatId = cId;
                userId = uId;
            });

            await connection.InvokeAsync("AddUserToChat", 1, TestData.User2.Id);

            chatId.Should().Be(1);
            userId.Should().Be(TestData.User2.Id);

            chatId = 0;
            userId = 0;
            connection.On<int, int>("DeleteUserFromChat", (uId, cId) => {
                chatId = cId;
                userId = uId;
            });

            await connection.InvokeAsync("DeleteUserFromChat",  1, TestData.User2.Id);

            chatId.Should().Be(1);
            userId.Should().Be(TestData.User2.Id);
        }
    }
}