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
        Task<Transaction?> GetById(Guid id);
        Task<List<Transaction>> GetAll();
        Task<List<Transaction>> GetByTarget(Guid targetId);
        Task Add(Transaction transaction);
    }
}
