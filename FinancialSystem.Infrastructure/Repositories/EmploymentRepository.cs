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
        public async Task AddAsync(Employment employment)
        {
            await _dbContext.Employments.AddAsync(employment);
        }
        public async Task<Employment?> GetByClientAndEnterpriseAsync(Guid clientId, Guid enterpriseId)
        {
            return await _dbContext.Employments.FirstOrDefaultAsync(emp => emp.ClientId == clientId && emp.EnterpriseId == enterpriseId);
        }
        public async Task<List<Employment>> GetByClientIdAsync(Guid clientId)
        {
            return await _dbContext.Employments
                                    .Where(emp => emp.ClientId == clientId)
                                    .ToListAsync();
        }
        public async Task<List<Employment>> GetByEnterpriseIdAsync(Guid enterpriseId)
        {
            return await _dbContext.Employments
                                    .Where(emp => emp.EnterpriseId == enterpriseId)
                                    .ToListAsync();
        }
        public async Task<bool> RemoveAsync(Guid id)
        {
            Employment? employment = await _dbContext.Employments.FindAsync(id);
            if (employment == null)
                return false;

            _dbContext.Employments.Remove(employment);
            return true;
        }
    }
}
