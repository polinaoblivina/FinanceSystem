using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface ILogRepository
    {
        Task<SystemLog?> GetById(Guid id);
        Task<List<SystemLog>> GetAll();
        Task Add(SystemLog log);
    }
}
