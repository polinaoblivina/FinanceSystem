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
        Task<Account?> GetById(Guid id);
        Task<List<Account>> GetByClientId(Guid clientId);
        Task Add(Account account);
        Task Update(Account account);
    }
}
