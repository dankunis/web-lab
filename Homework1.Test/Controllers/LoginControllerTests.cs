using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Homework1.Models.Requests;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using Homework1.Test.Extensions;
using Newtonsoft.Json.Linq;

namespace Homework1.Test.Controllers
{
    [Collection(ApiCollectionFixture.Name)]
    public class LoginControllerTests
    {
        public ApiFixture Api { get; set; }

        private ITestOutputHelper _output;
        private const string _uri = "api/v1/login";

        public LoginControllerTests(ApiFixture api, ITestOutputHelper output)
        {
            Api = api;
            _output = output;
        }

        [Fact]
        public async Task UserCanLogin()
        {
            var request = new LoginRequest
            {
               Email = TestData.User.Email,
               Password = TestData.User.Password
            };
            var response = await Api.Client.PostAsJsonAsync(_uri, request);

            response.StatusCode.Should().BeEquivalentTo(200);
            response.Headers.FirstOrDefault(x => x.Key.Equals("Set-Cookie")).Value.Should().NotBeNull();
        }

        [Theory]
        [InlineData("FALSE_LOGIN@gmail.com", "12345")]
        [InlineData("user@gmail.com", "FALSE_PASSWORD")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public async Task UserWithInvalidCredentialsCannotLogin(string email, string password)
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password,
            };

            var response = await Api.Client.PostAsJsonAsync(_uri, request);

            response.StatusCode.Should().BeEquivalentTo(400);
            response.Headers.FirstOrDefault(x => x.Key.Equals("Set-Cookie")).Value.Should().BeNull();
        }
    }
}