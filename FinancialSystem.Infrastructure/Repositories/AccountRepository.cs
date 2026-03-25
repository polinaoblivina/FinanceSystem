using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly FinanceDbContext _dbContext;
        public AccountRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Account account)
        {
            await _dbContext.Accounts.AddAsync(account);
        }

        public async Task<List<Account>> GetByClientIdAsync(Guid clientId)
        {
            return await _dbContext.Accounts
                            .Where(acc => acc.ClientId == clientId)
                            .ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Accounts.FindAsync(id);
        }

        public async Task<Account?> GetByNumberAsync(string number)
        {
            return await _dbContext.Accounts
                            .FirstOrDefaultAsync(a => a.Number == number);
        }
        public Task UpdateAsync(Account account)
        {
            _dbContext.Accounts.Update(account);
            return Task.CompletedTask;
        }
    }
}
