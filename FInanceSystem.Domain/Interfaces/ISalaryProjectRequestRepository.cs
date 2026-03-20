using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface ISalaryProjectRequestRepository
    {
        Task<SalaryProjectRequest?> GetByIdAsync(Guid id);
        Task<List<SalaryProjectRequest>> GetPendingAsync();
        Task<List<SalaryProjectRequest>> GetByClientIdAsync(Guid clientId);
        Task<SalaryProjectRequest?> GetByClientAndEnterpriseAsync(Guid clientId, Guid enterpriseId);
        Task AddAsync(SalaryProjectRequest request);
        Task UpdateAsync(SalaryProjectRequest request);
    }
}
