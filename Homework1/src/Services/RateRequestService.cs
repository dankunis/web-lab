using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Homework1.Database;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Qoden.Validation;

namespace Homework1.Services
{
    public interface IRateRequestService
    {
        Task<List<SalaryRateRequestResponse>> GetAllSalaryRateRequests();
        Task<List<SalaryRateRequestResponse>> GetSalaryRateRequestsForUsers(params int[] usersId);
        Task CreateSalaryRateRequest(int id, CreateRateRequestRequest request);
    }

    public class RateRequestService : IRateRequestService
    {
        private readonly IDbConnectionFactory _dbConnFactory;

        public RateRequestService(IDbConnectionFactory factory)
        {
            _dbConnFactory = factory;
        }

        public async Task<List<SalaryRateRequestResponse>> GetAllSalaryRateRequests()
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                return (await conn.QueryAsync<SalaryRateRequest>("SELECT * FROM salary_rate_requests")).Select(request =>
                    AutoMapper.Mapper.Map<SalaryRateRequest, SalaryRateRequestResponse>(request)).ToList();
            }
        }

        public async Task<List<SalaryRateRequestResponse>> GetSalaryRateRequestsForUsers(params int[] usersId)
        {
            var result = new List<SalaryRateRequestResponse>();

            using (var conn = _dbConnFactory.CreateConnection())
            {
                foreach (var userId in usersId)
                {
                    var requests = (await conn.QueryAsync<SalaryRateRequest>("SELECT * FROM salary_rate_requests where user_id=@UserId",
                        new {UserId = userId})).Select(request =>
                        AutoMapper.Mapper.Map<SalaryRateRequest, SalaryRateRequestResponse>(request)).ToList();

                    result.AddRange(requests);
                }

                return result;
            }
        }

        public async Task CreateSalaryRateRequest(int id, CreateRateRequestRequest request)
        {
            Check.Value(request, "Request").NotNull();
            request.Validate(ImmediateValidator.Instance);

            using (var conn = _dbConnFactory.CreateConnection())
            {
                var user = await conn.GetUserById(id);
                Check.Value(user, "Request").NotNull("User doesn't exist");

                await conn.ExecuteAsync("INSERT INTO salary_rate_requests (user_id, request_id, value_rate, description, " +
                                  "invited_at, status) VALUES (@UserId, @RequestId, @ValueRate, @Description, @InvitedAt, " +
                                  "@Status)", new SalaryRateRequest
                {
                    UserId = id,
                    RequestId = Guid.NewGuid(),
                    ValueRate = request.ValueRate,
                    Description = request.Description,
                    InvitedAt = DateTime.Now,
                    Status = 0
                });
            }
        }
    }
}