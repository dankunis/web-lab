using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Homework1.Database;
using Homework1.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Homework1.Services.Repositories
{
    public static class UserRepository
    {
        public static async Task<User> GetUserById(this IDbConnection db, int id)
        {
            //return new UserConnection(db, $"WHERE id='{id}' ")
            return await db.QueryFirstOrDefaultAsync<User>("select * from users where id=@Id", new {Id = id});
        }

        public static async Task<User> GetUserByEmail(this IDbConnection db, string email)
        {
            //return new UserConnection(db, $"WHERE email='{email}' ")
            return await db.QueryFirstOrDefaultAsync<User>("SELECT * FROM users WHERE email=@Email",
                new {Email = email});
        }

        // It's a good idea or not?
        // Use example:
        // (Login service: line 37)
        // var users = await conn.GetUserByEmail().WithRoles().Start();
        public class UserConnection
        {
            private IDbConnection _db;
            private string sql = "SELECT * FROM users ";
            private string conditions;

            public UserConnection(IDbConnection db, string initialConditions)
            {
                _db = db;
                conditions = initialConditions;
            }

            public UserConnection WithRoles()
            {
                sql += "INNER JOIN user_roles ON user_roles.user_id = users.id " +
                       "INNER JOIN roles ON roles.id = user_roles.role_id ";
                return this;
            }

            public UserConnection WithDepartment()
            {
                sql += "INNER JOIN departments ON departments.id = users.department_id ";
                return this;
            }

            public async Task<User> Start()
            {
                return await _db.QueryFirstOrDefaultAsync<User>(sql + conditions);
            }
        }
    }
}