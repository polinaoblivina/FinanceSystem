using System;
using FInanceSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<Transaction?> GetByNumberAsync(string number);
        Task<List<Transaction>> GetAllAsync();
        Task<List<Transaction>> GetByAccountIdAsync(Guid accountId);  
        Task AddAsync(Transaction transaction);
    }
}
