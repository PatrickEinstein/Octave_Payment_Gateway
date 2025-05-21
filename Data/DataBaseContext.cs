

using Microsoft.EntityFrameworkCore;

using CentralPG.Core.Models.Entities;
using OCPG.Core.Models.Entities;

namespace CentralPG.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {

        }


        public DbSet<AuthTokens> Auths { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PaymentTransactions> payment { get; set; }

        public DbSet<Cards> cards { get; set; }
       



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}