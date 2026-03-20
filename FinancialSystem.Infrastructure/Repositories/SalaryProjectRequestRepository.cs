using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Infrastructure.Repositories
{
    public class SalaryProjectRequestRepository: ISalaryProjectRequestRepository
    {
        private readonly FinanceDbContext _dbContext;
        public SalaryProjectRequestRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(SalaryProjectRequest request)
        {
            await _dbContext.SalaryProjectRequests.AddAsync(request);
        }
        public async Task<SalaryProjectRequest?> GetByClientAndEnterpriseAsync(Guid clientId, Guid enterpriseId)
        {
            return await _dbContext.SalaryProjectRequests
                                        .FirstOrDefaultAsync(req => req.EnterpriseId == enterpriseId && req.ClientId == clientId);
        }

        public async Task<List<SalaryProjectRequest>> GetByClientIdAsync(Guid clientId)
        {
            return await _dbContext.SalaryProjectRequests
                                        .Where(req => req.ClientId == clientId)
                                        .ToListAsync();
        }

        public async Task<SalaryProjectRequest?> GetByIdAsync(Guid id)
        {
            return await _dbContext.SalaryProjectRequests
                                        .FindAsync(id);
        }

        public async Task<List<SalaryProjectRequest>> GetPendingAsync()
        {
            return await _dbContext.SalaryProjectRequests
                                    .Where(req => req.Status == RequestStatus.Pending)
                                    .ToListAsync();
        }
        public Task UpdateAsync(SalaryProjectRequest request)
        {
            _dbContext.SalaryProjectRequests.Update(request);
            return Task.CompletedTask;
        }
    }
}
