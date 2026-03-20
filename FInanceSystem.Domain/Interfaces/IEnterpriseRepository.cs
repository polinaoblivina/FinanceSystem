using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface IEnterpriseRepository
    {
        Task<Enterprise?> GetByIdAsync(Guid id);
        Task<List<Enterprise>> GetAllAsync();
        Task AddAsync(Enterprise enterprise);
    }
}
