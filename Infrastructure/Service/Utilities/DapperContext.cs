

using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using CentralPG.Infrasturcture.Interfaces.Utilities;

namespace CentralPG.Infrastructure.Sevices.Utilities
{
    public class DapperContext : IDapperContext
    {
        private readonly IConfiguration configuration;

        public DapperContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IDbConnection GetDbConnection()
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            return new NpgsqlConnection(connectionString);
        }

        public IDbConnection GetMerchantDbConnection()
        {
            string connectionString = configuration.GetConnectionString("MerchantConnection");
            return new NpgsqlConnection(connectionString);
        }
        public IDbConnection GetPaymentDbConnection()
        {
            string connectionString = configuration.GetConnectionString("PaymentConnection");
            return new NpgsqlConnection(connectionString);
        }


    }
}