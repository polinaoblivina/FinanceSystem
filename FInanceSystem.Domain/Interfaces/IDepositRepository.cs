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
        Task<Deposit?> GetByIdAsync(Guid id);              
        Task<List<Deposit>> GetByClientIdAsync(Guid clientId);
        Task AddAsync(Deposit deposit);
        Task UpdateAsync(Deposit deposit);
    }
}
