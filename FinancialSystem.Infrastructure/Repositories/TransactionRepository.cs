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

        public async Task Add(Transaction transaction)
        {
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAll()
        {
            return await _dbContext.Transactions
                                    .ToListAsync();
        }

        public async Task<Transaction?> GetById(Guid id)
        {
            return await _dbContext.Transactions
                                    .FirstOrDefaultAsync(trans => trans.Id == id);
        }

        public Task<List<Transaction>> GetByTarget(Guid targetId)
        {
            // НАХУЯ
        }
    }
}
