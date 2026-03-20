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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly FinanceDbContext _dbContext;
        public TransactionRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _dbContext.Transactions.AddAsync(transaction);
        }
        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _dbContext.Transactions.ToListAsync();
        }
        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Transactions
                                    .FindAsync(id);
        }
        public async Task<List<Transaction>> GetByAccountIdAsync(Guid accountId)
        {
            return await _dbContext.Transactions
                                    .Where(t => t.FromId == accountId || t.ToId == accountId)
                                    .ToListAsync();
        }
    }
}
