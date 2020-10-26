using System.Linq;
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
    public class ManagementAreaControllerTests
    {
        public ApiFixture Api { get; set; }

        private ITestOutputHelper _output;
        private const string _uri = "api/v1/managementArea";

        public ManagementAreaControllerTests(ApiFixture api, ITestOutputHelper output)
        {
            Api = api;
            _output = output;
        }

        [Fact]
        public async Task AdminCanAssignAndCancelUserFromManager()
        {
            var request = new ManagementAreaRequest
            {
                UserEmail = TestData.User2.Email,
                ManagerEmail = TestData.Manager.Email
            };

            // assign
            var assignResponse = await Api.Client.WithRole(RoleNames.Admin).PostAsJsonAsync(
                _uri, request);
            assignResponse.StatusCode.Should().BeEquivalentTo(200);
            assignResponse.Content.Should().NotBeNull();

            // cancel
            var cancelResponse = await Api.Client.WithRole(RoleNames.Admin).DeleteAsJsonAsync(
                _uri, typeof(ManagementAreaRequest), request);
            cancelResponse.StatusCode.Should().BeEquivalentTo(200);
            cancelResponse.Content.Should().NotBeNull();
        }

        [Theory]
        [InlineData(RoleNames.User, 403)]
        [InlineData(RoleNames.Manager, 403)]
        [InlineData(null, 401)]
        public async Task AccessDeniedForUsersWithoutTheNecessaryRights(RoleNames? role, int code)
        {
            var request = new ManagementAreaRequest
            {
                UserEmail = TestData.User2.Email,
                ManagerEmail = TestData.Manager.Email
            };

            // assign
            var assignResponse = await Api.Client.WithRole(role).PostAsJsonAsync(
                _uri, request);
            assignResponse.StatusCode.Should().BeEquivalentTo(code);

            // cancel
            var cancelResponse = await Api.Client.WithRole(role).DeleteAsJsonAsync(
                _uri, typeof(ManagementAreaRequest), request);
            cancelResponse.StatusCode.Should().BeEquivalentTo(code);
        }

        [Theory]
        [InlineData("NotExisting@gmail.com", "NotExisting@gmail.com")]
        [InlineData(null, null)]
        [InlineData("Not email", "Not email")]
        public async Task AdminCannotAssignAndCancelInvalidUsers(string user, string manager)
        {
            var request = new ManagementAreaRequest
            {
                UserEmail = user,
                ManagerEmail = manager
            };

            // assign
            var assignResponse = await Api.Client.WithRole(RoleNames.Admin).PostAsJsonAsync(
                _uri, request);
            assignResponse.StatusCode.Should().BeEquivalentTo(400);

            // cancel
            var cancelResponse = await Api.Client.WithRole(RoleNames.Admin).DeleteAsJsonAsync(
                _uri, typeof(ManagementAreaRequest), request);
            cancelResponse.StatusCode.Should().BeEquivalentTo(400);
        }
    }
}