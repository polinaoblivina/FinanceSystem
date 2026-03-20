using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetById(Guid id);
        Task<User?> GetByLogin(string login);
        Task<List<User>> GetAll();
        Task Add(User user);
        Task Update(User user);
    }
}
