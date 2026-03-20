using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id);                    
        Task<List<Account>> GetByClientIdAsync(Guid clientId);   
        Task AddAsync(Account account);                          
        Task UpdateAsync(Account account);
    }
}
