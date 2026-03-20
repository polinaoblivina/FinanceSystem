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
        public async Task Add(Deposit deposit)
        {
            _dbContext.Deposits.Add(deposit);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Deposit>> GetByClientId(Guid clientId)
        {
            return await _dbContext.Deposits
                           .Where(dep => dep.ClientId == clientId)
                           .ToListAsync();
        }

        public async Task<Deposit?> GetById(Guid id)
        {
            return await _dbContext.Deposits
                           .FirstOrDefaultAsync(dep => dep.Id == id);
        }

        public async Task Update(Deposit deposit)
        {
            _dbContext.Deposits.Update(deposit);
            await _dbContext.SaveChangesAsync();
        }
    }
}
