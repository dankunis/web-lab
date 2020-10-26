using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Test.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Homework1.Test.Controllers
{
    [Collection(ApiCollectionFixture.Name)]
    public class AccountControllerTest
    {
        public ApiFixture Api { get; set; }

        private ITestOutputHelper _output;
        private const string _uri = "api/v1/account";

        public AccountControllerTest(ApiFixture api, ITestOutputHelper output)
        {
            Api = api;
            _output = output;
        }

        [Fact]
        public async Task AuthorizedUserCanGetProfile()
        {
            var correctProfile =  AutoMapper.Mapper.Map<User, ProfileResponse>(TestData.User);

            var response = await Api.Client.WithRole(RoleNames.User).GetAsync(_uri);
            var body = await response.Content.ReadAsStringAsync();
            var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<ProfileResponse>(body);

            response.StatusCode.Should().BeEquivalentTo(200);
            profile.Should().BeEquivalentTo(correctProfile);
        }

        [Fact]
        public async Task UnauthorizedUserCannotGetProfile()
        {
            var response = await Api.Client.WithRole(null).GetAsync(_uri);
            response.StatusCode.Should().BeEquivalentTo(401);
        }

        [Theory]
        [InlineData(RoleNames.Admin)]
        [InlineData(RoleNames.Manager)]
        public async Task UserWithNecessaryRightsCanGetUserInfo(RoleNames role)
        {
            var correctInfo = AutoMapper.Mapper.Map<User, UserInfoResponse>(TestData.User);

            var response = await Api.Client.WithRole(role).GetAsync(_uri + "/user/" + TestData.User.Id);
            var body = await response.Content.ReadAsStringAsync();
            var info = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoResponse>(body);

            response.StatusCode.Should().BeEquivalentTo(200);
            info.Should().BeEquivalentTo(correctInfo);
        }

        [Theory]
        [InlineData(RoleNames.User, 403)]
        [InlineData(null, 401)]
        public async Task UserWithoutNecessaryRightsCannotGetUserInfo(RoleNames? role, int code)
        {
            var response = await Api.Client.WithRole(role).GetAsync(_uri + "/user/" + TestData.User.Id);
            response.StatusCode.Should().BeEquivalentTo(code);
        }

        [Fact]
        public async Task UserWithNecessaryRightsCanCreateUser()
        {
            var request = new CreateUserRequest
            {
                FirstName = "user",
                LastName = "user",
                Patronymic = "user",
                NickName = TestData.RandomString(12),
                Email = TestData.RandomString(12) + "@gmail.com",
                Password = "123456789",
                PhoneNumber = "0-000-000-00-00",
                Description = "I'm user",
                DepartmentName = "developers"
            };

            var response = await Api.Client.WithRole(RoleNames.Admin).PostAsJsonAsync(_uri, request);
            response.StatusCode.Should().BeEquivalentTo(200);

            var check = new LoginRequest
            {
                Email = request.Email,
                Password = request.Password
            };

            var response2 = await Api.Client.PostAsJsonAsync("api/v1/login", check);
            response2.StatusCode.Should().BeEquivalentTo(200);
        }

        [Theory]
        [InlineData("user2", "email@mail.ru", "123456789", "developers")] // Exist nickname
        [InlineData("Bla", "user@gmail.com", "123456789", "developers")] // Exist email
        [InlineData("Bla", "bdbfdfd@gmail.ru", "1234", "developers")] // Password length less then 8
        [InlineData("Bla", "blfdgv@gmail.ru", "1234568972", "BLA")] // Invalid department name
        public async Task UserWithNecessaryRightsCannotCreateUserWithInvalidData(string nickName, string email,
            string password, string department)
        {
            var request = new CreateUserRequest
            {
                FirstName = "user",
                LastName = "user",
                Patronymic = "user",
                NickName = nickName,
                Email = email,
                Password = password,
                PhoneNumber = "0-000-000-00-00",
                Description = "I'm user",
                DepartmentName = department
            };

            var response = await Api.Client.WithRole(RoleNames.Admin).PostAsJsonAsync(_uri, request);
            response.StatusCode.Should().BeEquivalentTo(400);
        }

        [Theory]
        [InlineData(RoleNames.User, 403)]
        [InlineData(RoleNames.Manager, 403)]
        [InlineData(null, 401)]
        public async Task UserWithoutNecessaryRightsCannotCreateUser(RoleNames? role, int code)
        {
            var request = new CreateUserRequest();

            var response = await Api.Client.WithRole(role).PostAsJsonAsync(_uri, request);
            response.StatusCode.Should().BeEquivalentTo(code);
        }

        [Theory]
        [InlineData(RoleNames.Admin)]
        [InlineData(RoleNames.Manager)]
        public async Task UserWithNecessaryRightsCanModifyUser(RoleNames role)
        {
            var request = AutoMapper.Mapper.Map<User, UpdateUserInfoRequest>(TestData.ModifyUser());

            var response = await Api.Client.WithRole(role).PatchAsJsonAsync(_uri + "/user/36",
                typeof(UpdateUserInfoRequest), request);
            response.StatusCode.Should().BeEquivalentTo(200);

            var body = await response.Content.ReadAsStringAsync();
            var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoResponse>(body);

            profile.Should().BeEquivalentTo(request);
        }

        [Fact]
        public async Task ManagerCannotModifyUnavailableUser()
        {
            var request = new UpdateUserInfoRequest();

            var response = await Api.Client.WithRole(RoleNames.Manager).PatchAsJsonAsync(_uri + "/user/21",
                typeof(UpdateUserInfoRequest), request);

            response.StatusCode.Should().BeEquivalentTo(403);
        }

        [Theory]
        [InlineData(RoleNames.User, 403)]
        [InlineData(null, 401)]
        public async Task UserWithoutNecessaryRightsCannotModifyUser(RoleNames? role, int code)
        {
            var request = new UpdateUserInfoRequest();

            var response = await Api.Client.WithRole(role).PatchAsJsonAsync(_uri + "/user/6",
                typeof(UpdateUserInfoRequest), request);
            response.StatusCode.Should().BeEquivalentTo(code);
        }

        [Theory]
        [InlineData("user2", "email@mail.ru")] // Exist nickname
        [InlineData("Bla", "user@gmail.com")] // Exist email
        public async Task UserWithNecessaryRightsCannotModifyUserWithInvalidData(string nickName, string email)
        {
            var request = AutoMapper.Mapper.Map<User, UpdateUserInfoRequest>(TestData.ModifyUser());
            request.NickName = nickName;
            request.Email = email;

            var response = await Api.Client.WithRole(RoleNames.Admin).PatchAsJsonAsync(_uri + "/user/6",
                typeof(UpdateUserInfoRequest), request);
            response.StatusCode.Should().BeEquivalentTo(400);
        }
    }
}