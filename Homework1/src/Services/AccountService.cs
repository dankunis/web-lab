using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Homework1.Database;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Models.Responses;
using Homework1.Services.Repositories;
using Microsoft.AspNetCore.Identity;
using Qoden.Validation;

namespace Homework1.Services
{
    public interface IAccountService
    {
        Task<ProfileResponse> GetProfile(int id);
        Task<UserInfoResponse> ModifyUser(UpdateUserInfoRequest request, int userId);
        Task<UserInfoResponse> GetUserInfo(int id);
        Task CreateUser(CreateUserRequest request);
    }

    public class AccountService : IAccountService
    {
        private readonly IDbConnectionFactory _dbConnFactory;

        public AccountService(IDbConnectionFactory factory)
        {
            _dbConnFactory = factory;
        }

        public async Task<ProfileResponse> GetProfile(int id)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                var user = await conn.GetUserById(id);
                Check.Value(user, "Request").NotNull("User doesn't exist");

                return AutoMapper.Mapper.Map<User, ProfileResponse>(user);
            }
        }

        public async Task<UserInfoResponse> ModifyUser(UpdateUserInfoRequest request, int userId)
        {
            Check.Value(request, "Request").NotNull();
            request.Validate(ImmediateValidator.Instance);

            using (var conn = _dbConnFactory.CreateConnection())
            {
                var uniqueEmail = await conn.QueryFirstOrDefaultAsync<string>("SELECT email FROM users WHERE " +
                                                                              $"email='{request.Email}' AND id<>'{userId}'");
                Check.Value(uniqueEmail, "Request").IsNull("This email already exist");

                var uniqueNickName = await conn.QueryFirstOrDefaultAsync<string>("SELECT nick_name FROM users WHERE " +
                                                            $"nick_name='{request.NickName}' AND id<>'{userId}'");
                Check.Value(uniqueNickName, "Request").IsNull("This email already exist");

                var dbUser = await conn.GetUserById(userId);
                Check.Value(dbUser).NotNull("User doesn't exist");

                conn.Execute("UPDATE users SET first_name=@FirstName, last_name=@LastName, patronymic=@Patronymic, " +
                             $"nick_name=@NickName, email=@Email, phone_number=@PhoneNumber, description=@Description where id='{userId}'", request);
                return AutoMapper.Mapper.Map<UpdateUserInfoRequest, UserInfoResponse>(request);
            }
        }

        public async Task<UserInfoResponse> GetUserInfo(int id)
        {
            using (var conn = _dbConnFactory.CreateConnection())
            {
                var user = await conn.GetUserById(id);
                Check.Value(user, "Request").NotNull("User doesn't exist");

                return AutoMapper.Mapper.Map<User, UserInfoResponse>(user);
            }
        }

        public async Task CreateUser(CreateUserRequest request)
        {
            Check.Value(request, "Request").NotNull();
            request.Validate(ImmediateValidator.Instance);

            using (var conn = _dbConnFactory.CreateConnection())
            {
                var uniqueEmail = await conn.QueryFirstOrDefaultAsync<string>("select email from users where " +
                                                                              "email=@Email", new {Email = request.Email});
                Check.Value(uniqueEmail, "Request").IsNull("This email already exist");

                var uniqueNickName = await conn.QueryFirstOrDefaultAsync<string>("SELECT nick_name FROM users WHERE " +
                                                                                 $"nick_name='{request.NickName}'");
                Check.Value(uniqueNickName, "Request").IsNull("This email already exist");

                var department = await conn.QueryFirstOrDefaultAsync<Department>("SELECT * FROM departments WHERE " +
                                                            $"name='{request.DepartmentName}'");
                Check.Value(department, "Request").NotNull("Department name doesn't exist");

                var user = AutoMapper.Mapper.Map<CreateUserRequest, User>(request);

                user.InvitedAt = DateTime.Now;
                user.Password = HashPassword(user, request.Password);
                user.CurrentSalaryRateId = null;
                user.DepartmentId = department.Id;

                var id = await conn.QueryAsync<int>(
                    "INSERT INTO users (first_name, last_name, patronymic, nick_name, email, password, phone_number, invited_at, description, department_id, current_salary_rate_id) " +
                    "VALUES (@FirstName, @LastName, @Patronymic, @NickName, @Email, @Password, @PhoneNumber, @InvitedAt, @Description, @DepartmentId, @CurrentSalaryRateId); " +
                    "SELECT lastval(); ", user);
                await conn.ExecuteAsync($"INSERT INTO user_roles (user_id, role_id) VALUES ('{id.First()}', '{1}');");
            }
        }

        private static string HashPassword(User user, string password)
        {
            return new PasswordHasher<User>().HashPassword(user, password);
        }
    }
}