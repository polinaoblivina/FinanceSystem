using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FInanceSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;

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

        public FinanceDbContext()
            :base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}


