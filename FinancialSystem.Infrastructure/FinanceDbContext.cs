using Microsoft.EntityFrameworkCore;
using FInanceSystem.Domain.Entities;

namespace FinanceSystem.Infrastructure
{
    public class FinanceDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SystemLog> Logs { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<SalaryProjectRequest> SalaryProjectRequests { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
            : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Client>("Client")
                .HasValue<Admin>("Admin")
                .HasValue<Manager>("Manager");
        }
    }
}
