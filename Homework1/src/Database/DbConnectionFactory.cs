using System;
using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Homework1.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
        string GetConnectionString();
    }

    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private DbOptions _options;

        public DbConnectionFactory(IOptions<DbOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException("DbOptions");
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_options.ConnectionString);
        }

        public string GetConnectionString()
        {
            return _options.ConnectionString;
        }
    }
}