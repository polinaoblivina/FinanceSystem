using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface IEmploymentRepository
    {
        Task<List<Employment>> GetByEnterpriseIdAsync(Guid enterpriseId);
        Task<List<Employment>> GetByClientIdAsync(Guid clientId);
        Task<Employment?> GetByClientAndEnterpriseAsync(Guid clientId, Guid enterpriseId);
        Task AddAsync(Employment employment);
        Task<bool> RemoveAsync(Guid id);
    }
}
