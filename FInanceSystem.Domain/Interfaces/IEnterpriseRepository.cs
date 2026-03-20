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
        Task<Enterprise?> GetById(Guid id);
        Task<List<Enterprise>> GetAll();
        Task Add(Enterprise enterprise);
    }
}
