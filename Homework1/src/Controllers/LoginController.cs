using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Homework1.Models.Requests;
using Homework1.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Homework1.Controllers
{
    [Route("api/v1")]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        public LoginController([FromServices] ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task Login([FromBody] LoginRequest request)
        {
            var id = await _loginService.Login(request);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}