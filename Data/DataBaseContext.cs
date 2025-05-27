

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
        public DbSet<PaymentTransactions> Payment { get; set; }
        public DbSet<Cards> Cards { get; set; }
        public DbSet<Wallets> Wallets { get; set; }
        public DbSet<WalletTransactionHistory> WalletTransactionHistory { get; set; }
        public DbSet<Withdrawals> Withdrawals { get; set; }
       



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}