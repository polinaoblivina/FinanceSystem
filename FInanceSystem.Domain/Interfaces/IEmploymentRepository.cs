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
        Task<List<Employment>> GetByEnterpriseId(Guid enterpriseId);
        Task<List<Employment>> GetByClientId(Guid clientId);
        Task<Employment?> GetByClientAndEnterprise(Guid clientId, Guid enterpriseId);
        Task Add(Employment employment);
        Task<bool> Remove(Guid id);
    }
}
