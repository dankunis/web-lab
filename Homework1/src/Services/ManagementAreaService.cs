using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Homework1.Database;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Qoden.Validation;

namespace Homework1.Services
{
    public interface IManagementAreaService
    {
        Task AssignUserToManager(ManagementAreaRequest request);
        Task CancelUserFromManager(ManagementAreaRequest request);
        Task<List<int>> GetAvailableUsersIdForManager(int managerId);
    }

    public class ManagementAreaService : IManagementAreaService
    {
        private readonly IDbConnectionFactory _dbConnFactory;

        public ManagementAreaService(IDbConnectionFactory factory)
        {
            _dbConnFactory = factory;
        }

        public async Task AssignUserToManager(ManagementAreaRequest request)
        {
            Check.Value(request, "Request").NotNull();
            request.Validate(ImmediateValidator.Instance);

            using (var conn = _dbConnFactory.CreateConnection())
            {
                var user = await conn.GetUserByEmail(request.UserEmail);
                var manager = await conn.GetUserByEmail(request.ManagerEmail);

                Check.Value(user).NotNull("User doesn't exist");
                Check.Value(manager).NotNull("Manager doesn't exist");

                var checkExist = await conn.QueryFirstOrDefaultAsync<ManagementArea>("select * from management_areas " +
                                                  $"where user_id='{user.Id}' and manager_id='{manager.Id}'");
                if (checkExist != null)
                {
                    checkExist.User = null;
                    checkExist.Manager = null;
                }

                Check.Value(checkExist).IsNull("This user already belongs to this manager");

                await conn.ExecuteAsync(
                    "INSERT INTO management_areas (user_id, manager_id) VALUES (@UserId, @ManagerId)",
                    new ManagementArea()
                    {
                        UserId = user.Id,
                        ManagerId = manager.Id
                    });
            }
        }

        public async Task<List<int>> GetAvailableUsersIdForManager(int managerId)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                var manager = await conn.GetUserById(managerId);
                Check.Value(manager).NotNull("Manager doesn't exist");

                return (await conn.QueryAsync<int>("SELECT user_id FROM management_areas where manager_id=@Id", new {Id = managerId}))
                    .ToList();
            }
        }

        public async Task CancelUserFromManager(ManagementAreaRequest request)
        {
            Check.Value(request, "Request").NotNull();
            request.Validate(ImmediateValidator.Instance);

            using (var conn = _dbConnFactory.CreateConnection())
            {
                var user = await conn.GetUserByEmail(request.UserEmail);
                var manager = await conn.GetUserByEmail(request.ManagerEmail);

                Check.Value(user).NotNull("User doesn't exist");
                Check.Value(manager).NotNull("Manager doesn't exist");

                var checkExist = await conn.QueryFirstOrDefaultAsync<ManagementArea>("select * from management_areas " +
                                                            $"where user_id='{user.Id}' and manager_id='{manager.Id}'");
                Check.Value(checkExist).NotNull("This user already unassigned to this manager");

                await conn.ExecuteAsync($"DELETE FROM management_areas WHERE user_id='{checkExist.UserId}' " +
                                        $"and manager_id='{checkExist.ManagerId}'");
            }
        }
    }
}