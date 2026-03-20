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

        public async Task Add(SalaryProjectRequest request)
        {
            _dbContext.SalaryProjectRequests.Add(request);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SalaryProjectRequest?> GetApproved(Guid clientId, Guid enterpriseId)
        {
            // ВОПРОСЫ
        }

        public async Task<SalaryProjectRequest?> GetByClientAndEnterprise(Guid clientId, Guid enterpriseId)
        {
            return await _dbContext.SalaryProjectRequests
                                    .FirstOrDefaultAsync(req => req.EnterpriseId == enterpriseId && req.ClientId == clientId);
        }

        public async Task<List<SalaryProjectRequest>> GetByClientId(Guid clientId)
        {
            return await _dbContext.SalaryProjectRequests
                                        .Where(req => req.ClientId == clientId).ToListAsync();
        }

        public async Task<SalaryProjectRequest?> GetById(Guid id)
        {
            return await _dbContext.SalaryProjectRequests
                                        .FirstOrDefaultAsync(req => req.Id == id);
        }

        public async Task<List<SalaryProjectRequest>> GetPending()
        {
            return await _dbContext.SalaryProjectRequests
                                        .Where(req => req.Status == RequestStatus.Pending) // НАХУЯ
                                        .ToListAsync();
        }

        public async Task Update(SalaryProjectRequest request)
        {
            _dbContext.SalaryProjectRequests.Update(request);
            await _dbContext.SaveChangesAsync();
        }
    }
}
