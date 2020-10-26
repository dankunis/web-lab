using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Homework1.Hubs
{
    public class BaseHub : Hub
    {
        protected int UserId => int.Parse(Context.User.Identity.Name);
        protected string UserLogin => Context.User.Claims.FirstOrDefault( x =>
            x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
    }
}