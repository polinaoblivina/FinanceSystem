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
    public class EnterpriseRepository : IEnterpriseRepository
    {
        private readonly FinanceDbContext _dbContext;

        public EnterpriseRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Enterprise enterprise)
        {
            await _dbContext.Enterprises.AddAsync(enterprise);
        }
        public async Task<List<Enterprise>> GetAllAsync()
        {
            return await _dbContext.Enterprises.ToListAsync();
        }
        public async Task<Enterprise?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Enterprises
                                    .FindAsync(id);
        }
    }
}
