using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using CentralPG.Interfaces;
using CentralPG.Infrasturcture.Interfaces.Utilities;
using CentralPG.Infrasturcture.Interfaces.IRepositories;

namespace CentralPG.Infrastructure.Services.Repositories
{
    public class TestRespository:ITestRepository
    {
        private readonly IDapperContext dapperContext;

        public TestRespository(IDapperContext dapperContext)
        {
            this.dapperContext = dapperContext;
        }

        public async Task<List<T>> TestGeManyt<T>(string foo)
        {
            string query = "SELECT * FROM \"tableName\" WHERE \"tableColumn\" = @foo";
            using (var connection = dapperContext.GetPaymentDbConnection())
            {
                var result = await connection.QueryAsync<T>(query, new { foo });
                return result.ToList();
            }
        }
        public async Task<T> TestGetSingle<T>(string foo)
        {
            string query = "SELECT * FROM \"tableName\" WHERE \"tableColumn\" = @foo";
            using (var connection = dapperContext.GetMerchantDbConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<T>(query, new { foo });
                return result;
            }
        }

        public async Task<bool> TestUpdate(string foo)
        {
            string query = "UPDATE \"tableName\" SET \"foo\" = @foo";
            using (var connection = dapperContext.GetPaymentDbConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { foo });
                return affectedRows > 0;
            }
        }
    }
}