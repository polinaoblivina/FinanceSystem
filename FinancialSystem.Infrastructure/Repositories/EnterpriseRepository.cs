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
        public async Task Add(Enterprise enterprise)
        {
            _dbContext.Enterprises.Add(enterprise);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Enterprise>> GetAll()
        {
            return await _dbContext.Enterprises
                            .ToListAsync();
        }

        public async Task<Enterprise?> GetById(Guid id)
        {
            return await _dbContext.Enterprises
                               .FirstOrDefaultAsync(ent => ent.Id == id);
        }
    }
}
