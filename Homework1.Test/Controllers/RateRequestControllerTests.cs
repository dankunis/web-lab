using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Test.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Homework1.Test.Controllers
{
    [Collection(ApiCollectionFixture.Name)]
    public class RateRequestControllerTests
    {
        public ApiFixture Api { get; set; }

        private ITestOutputHelper _output;
        private const string _uri = "api/v1/salaryRates";

        public RateRequestControllerTests(ApiFixture api, ITestOutputHelper output)
        {
            Api = api;
            _output = output;
        }

        [Fact]
        public async Task AdminCanGetAllRequests()
        {
            var response = await Api.Client.WithRole(RoleNames.Admin).GetAsync(_uri + "/requests");
            response.StatusCode.Should().BeEquivalentTo(200);
            response.Content.Should().NotBeNull();
        }

        [Theory]
        [InlineData(RoleNames.User, 403)]
        [InlineData(RoleNames.Manager, 403)]
        [InlineData(null, 401)]
        public async Task UserWithoutNecessaryRightsCannotGetAllRequests(RoleNames? role, int code)
        {
            var response = await Api.Client.WithRole(role).GetAsync(_uri + "/requests");
            response.StatusCode.Should().BeEquivalentTo(code);
        }

        [Fact]
        public async Task UserCanGetRequests()
        {
            var response = await Api.Client.WithRole(RoleNames.User).GetAsync(_uri + "/user/requests");
            response.StatusCode.Should().BeEquivalentTo(200);
            response.Content.Should().NotBeNull();
        }

        [Fact]
        public async Task ManagerCanGetRequestsForAvailableUsers()
        {
            var response = await Api.Client.WithRole(RoleNames.Manager).GetAsync(_uri + "/assignedUsers/requests");
            response.StatusCode.Should().BeEquivalentTo(200);
            response.Content.Should().NotBeNull();
        }

        [Fact]
        public async Task UserCanCreateRequest()
        {
            var request = new CreateRateRequestRequest
            {
                ValueRate = 10000,
                Description = "Bla bla bla"
            };

            var response = await Api.Client.WithRole(RoleNames.User).PostAsJsonAsync(_uri + "/request", request);
            response.StatusCode.Should().BeEquivalentTo(200);
            response.Content.Should().NotBeNull();
        }

        [Theory]
        [InlineData(-1, "Bla")]
        [InlineData(0, "Bla")]
        [InlineData(null, "Bla")]
        [InlineData(10, null)]
        [InlineData(10, "")]
        public async Task UserCannotCreateRequestWithInvalidData(decimal valueRate, string description)
        {
            var request = new CreateRateRequestRequest
            {
                ValueRate = valueRate,
                Description = description
            };

            var response = await Api.Client.WithRole(RoleNames.User).PostAsJsonAsync(_uri + "/request", request);
            response.StatusCode.Should().BeEquivalentTo(400);
            response.Content.Should().NotBeNull();
        }
    }
}