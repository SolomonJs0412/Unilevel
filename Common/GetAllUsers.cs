using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Unilever.v1.Database.config;

namespace Unilever.v1.Common
{
    public class GetAllUsers : IGetAllUsers
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;
        private readonly string _connectionString;

        public GetAllUsers(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("DefaultCons");
        }

        public async Task<List<object>> GetAllUser()
        {
            // var query = $"SELECT * FROM {tableName} WHERE CONTAINS(*, '{searchTerm}')";
            var searchQuery = @"SELECT * FROM [User]";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(searchQuery, connection);

            var results = new List<object>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var result = new ExpandoObject() as IDictionary<string, object>;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    result[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(result);
            }

            return results;
        }

        public Task<List<object>> SearchUser()
        {
            throw new NotImplementedException();
        }
    }
}