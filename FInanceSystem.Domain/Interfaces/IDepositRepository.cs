using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface IDepositRepository
    {
        Task<Deposit> GetById(Guid id);
        Task<List<Deposit>> GetByClientId(Guid clientId);
        Task Add(Deposit deposit);
        Task Update(Deposit deposit);
    }
}
