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
    public class EmploymentRepository : IEmploymentRepository 
    {
        private readonly FinanceDbContext _dbContext;

        public EmploymentRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Employment employment)
        {
            _dbContext.Employments.Add(employment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Employment?> GetByClientAndEnterprise(Guid clientId, Guid enterpriseId)
        {
            return await _dbContext.Employments
                            .FirstOrDefaultAsync(emp => emp.ClientId == clientId && emp.EnterpriseId == enterpriseId);
        }

        public async Task<List<Employment>> GetByClientId(Guid clientId)
        {
            return await _dbContext.Employments
                            .Where(emp => emp.ClientId == clientId)
                            .ToListAsync();
        }

        public async Task<List<Employment>> GetByEnterpriseId(Guid enterpriseId)
        {
            return await _dbContext.Employments
                                    .Where(emp => emp.EnterpriseId == enterpriseId)
                                    .ToListAsync();
        }

        public async Task<bool> Remove(Guid id)
        {
            Employment? employment = await _dbContext.Employments.FindAsync(id);
            if (employment == null)
                return false;

            _dbContext.Employments.Remove(employment);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
