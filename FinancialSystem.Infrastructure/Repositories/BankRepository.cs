using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Infrastructure.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly FinanceDbContext _dbContext;

        public BankRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Bank bank)
        {
            await _dbContext.Banks.AddAsync(bank);
        }

        public async Task<List<Bank>> GetAllAsync()
        {
            return await _dbContext.Banks.ToListAsync();
        }

        public async Task<Bank?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Banks.FindAsync(id);
        }

        public async Task<Bank?> GetByNameAsync(string name)
        {
            return await _dbContext.Banks.FirstOrDefaultAsync(b => b.Name == name);
        }
    }
}
