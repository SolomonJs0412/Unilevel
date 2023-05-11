using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Unilever.v1.Common.SearchService
{
    public class SqlSearchService : ISearchService
    {
        private readonly string _connectionString;

        public SqlSearchService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultCons");
        }

        public async Task<List<object>> SearchUser(string searchTerm)
        {
            // var query = $"SELECT * FROM {tableName} WHERE CONTAINS(*, '{searchTerm}')";
            var searchQuery = @"SELECT * FROM [User]
                                WHERE Name LIKE {0}
                                OR Email LIKE {0}
                                OR AreaCd LIKE {0};";


            string queryString = String.Format(searchQuery, searchTerm);
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(queryString, connection);

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

        public async Task<List<object>> SearchArea(string tableName, string searchTerm)
        {
            // var query = $"SELECT * FROM {tableName} WHERE CONTAINS(*, '{searchTerm}')";
            var searchQuery = @"SELECT * FROM {0}
                                WHERE Name LIKE '%' + {1} + '%'
                                OR Email LIKE '%' + {1} + '%'
                                OR AreaCd LIKE '%' + {1} + '%';";


            string queryString = String.Format(searchQuery, tableName, searchTerm);
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(queryString, connection);

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
    }

}