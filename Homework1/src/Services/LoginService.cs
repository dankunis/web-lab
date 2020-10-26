using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Homework1.Database;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Homework1.Services.Repositories;
using Microsoft.AspNetCore.Identity;
using Qoden.Validation;

namespace Homework1.Services
{
    public interface ILoginService
    {
        Task<ClaimsIdentity> Login(LoginRequest request);
    }

    public class LoginService : ILoginService
    {
        private readonly IDbConnectionFactory _dbConnFactory;

        public LoginService(IDbConnectionFactory factory)
        {
            _dbConnFactory = factory;
        }

        public async Task<ClaimsIdentity> Login(LoginRequest request)
        {
            Check.Value(request, "Request").NotNull();
            request.Validate(ImmediateValidator.Instance);

            User user = null;
            using (var conn = _dbConnFactory.CreateConnection())
            {
                var users = await conn.QueryAsync<User, UserRole, Role, User>(
                    "SELECT * FROM users " +
                    "INNER JOIN user_roles ON user_roles.user_id = users.id " +
                    "INNER JOIN roles ON roles.id = user_roles.role_id " +
                    "WHERE users.email=@Email",
                    (u, ur, r) =>
                    {
                        if (user == null)
                        {
                            user = u;
                            user.UserRoles = new List<UserRole>();
                        }

                        ur.Role = r;
                        user.UserRoles.Add(ur);
                        return u;
                    },
                    new {Email = request.Email}
                );
                const string msg = "Invalid username or password";

                Check.Value(user, "credentials").NotNull(msg);

                var hash = new PasswordHasher<User>();

                Check.Value(hash.VerifyHashedPassword(user, user.Password, request.Password),
                    "credentials").EqualsTo(PasswordVerificationResult.Success, msg);

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                claims.AddRange(user.UserRoles.Select(userRole => new Claim(ClaimsIdentity.DefaultRoleClaimType,
                    userRole.Role.Name.ToString())));

                return new ClaimsIdentity(claims, "ApplicationCookie", ClaimTypes.NameIdentifier,
                    ClaimsIdentity.DefaultRoleClaimType);
            }
        }
    }
}