using System.Threading.Tasks;
using Homework1.Extensions;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qoden.Validation;

namespace Homework1.Controllers
{
    [Route("api/v1/account")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController([FromServices] IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task CreateUser([FromBody] CreateUserRequest request)
        {
            await _accountService.CreateUser(request);
        }

        [HttpPatch("user/{userId}")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<UserInfoResponse> ModifyUser([FromBody] UpdateUserInfoRequest request, [FromRoute] int userId,
            [FromServices] IManagementAreaService managementAreaService)
        {
            if (User.IsInRole("Manager"))
            {
                var users = await managementAreaService.GetAvailableUsersIdForManager(this.GetUserId());
                if (!users.Contains(userId))
                {
                    Response.StatusCode = 403;
                    return null;
                }
            }

            return await _accountService.ModifyUser(request, userId);
        }

        [HttpGet]
        public async Task<ProfileResponse> GetProfile()
        {
            return await _accountService.GetProfile(this.GetUserId());
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<UserInfoResponse> GetUserInfo([FromRoute] int userId)
        {
            return await _accountService.GetUserInfo(userId);
        }
    }
}