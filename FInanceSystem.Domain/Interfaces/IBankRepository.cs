using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface IBankRepository
    {
        Task<Bank?> GetById(Guid id);
        Task<List<Bank>> GetAll();
        Task Add(Bank bank);
    }
}
