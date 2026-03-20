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
        public async Task Add(Bank bank)
        {
            _dbContext.Banks.Add(bank);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Bank>> GetAll()
        {
            return await _dbContext.Banks.ToListAsync();
        }

        public Task<Bank?> GetById(Guid id)
        {
            return _dbContext.Banks
                            .FirstOrDefaultAsync(bank =>  bank.Id == id);
        }
    }
}
