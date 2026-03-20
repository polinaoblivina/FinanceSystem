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
        Task<SalaryProjectRequest?> GetById(Guid id);
        Task<List<SalaryProjectRequest>> GetPending();
        Task<List<SalaryProjectRequest>> GetByClientId(Guid clientId);
        Task<SalaryProjectRequest?> GetByClientAndEnterprise(Guid clientId, Guid enterpriseId);
        Task<SalaryProjectRequest?> GetApproved(Guid clientId, Guid enterpriseId);
        Task Add(SalaryProjectRequest request);
        Task Update(SalaryProjectRequest request);
    }
}
