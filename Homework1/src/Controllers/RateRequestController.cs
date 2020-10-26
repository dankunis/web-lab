using System.Collections.Generic;
using System.Threading.Tasks;
using Homework1.Extensions;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Homework1.Controllers
{
    [Route("api/v1/salaryRates")]
    [Authorize]
    public class FinancialController : Controller
    {
        private readonly IRateRequestService _rateRequestService;

        public FinancialController([FromServices] IRateRequestService rateRequestService)
        {
            _rateRequestService = rateRequestService;
        }

        [HttpGet("requests")]
        [Authorize(Roles = "Admin")]
        public async Task<List<SalaryRateRequestResponse>> GetAllRequests()
        {
            return await _rateRequestService.GetAllSalaryRateRequests();
        }

        [HttpGet("user/requests")]
        [Authorize(Roles = "User")]
        public async Task<List<SalaryRateRequestResponse>> GetUserRequests()
        {
            return await _rateRequestService.GetSalaryRateRequestsForUsers(this.GetUserId());
        }

        [HttpGet("assignedUsers/requests")]
        [Authorize(Roles = "Manager")]
        public async Task<List<SalaryRateRequestResponse>> GetAssignedUsers(
            [FromServices] IManagementAreaService managementAreaService)
        {
            var usersId = await managementAreaService.GetAvailableUsersIdForManager(this.GetUserId());

            return await _rateRequestService.GetSalaryRateRequestsForUsers(usersId.ToArray());
        }

        [HttpPost("request")]
        [Authorize(Roles = "User")]
        public async Task CreateRequest([FromBody] CreateRateRequestRequest request)
        {
            await _rateRequestService.CreateSalaryRateRequest(this.GetUserId(), request);
        }
    }
}