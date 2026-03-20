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
    public class DepositRepository : IDepositRepository
    {
        private readonly FinanceDbContext _dbContext;

        public DepositRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Deposit deposit)
        {
            await _dbContext.Deposits.AddAsync(deposit);
        }
        public async Task<List<Deposit>> GetByClientIdAsync(Guid clientId)
        {
            return await _dbContext.Deposits
                                    .Where(dep => dep.ClientId == clientId)
                                    .ToListAsync();
        }
        public async Task<Deposit?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Deposits.FindAsync(id);
        }
        public Task UpdateAsync(Deposit deposit)
        {
            _dbContext.Deposits.Update(deposit);
            return Task.CompletedTask;
        }
    }
}
