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
        public async Task Add(Account account)
        {
            _dbContext.Add(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Account>> GetByClientId(Guid clientId)
        {
            return await _dbContext.Accounts
                            .Where(acc => acc.ClientId == clientId)
                            .ToListAsync();
        }

        public async Task<Account?> GetById(Guid id)
        {
            return await _dbContext.Accounts.
                            FirstOrDefaultAsync(acc => acc.Id == id);
        }

        public async Task Update(Account account)
        {
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
        }
    }
}
