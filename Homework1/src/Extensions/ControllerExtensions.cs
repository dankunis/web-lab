using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Homework1.Extensions
{
    public static class ControllerExtensions
    {
        public static int GetUserId(this Controller controller)
        {
            return int.Parse(
                controller.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value
            );
        }
    }
}