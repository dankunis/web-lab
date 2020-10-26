using System.Threading.Tasks;
using Homework1.Models.Requests;
using Homework1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Homework1.Controllers
{
    [Route("api/v1/managementArea")]
    [Authorize(Roles = "Admin")]
    public class ManagementAreaController : Controller
    {
        private readonly IManagementAreaService _managementAreaService;

        public ManagementAreaController([FromServices] IManagementAreaService managementAreaService)
        {
            _managementAreaService = managementAreaService;
        }

        [HttpPost]
        public async Task AssignUserToManager([FromBody] ManagementAreaRequest request)
        {
             await _managementAreaService.AssignUserToManager(request);
        }

        [HttpDelete]
        public async Task CancelUserFromManager([FromBody] ManagementAreaRequest request)
        {
            await _managementAreaService.CancelUserFromManager(request);
        }
    }
}