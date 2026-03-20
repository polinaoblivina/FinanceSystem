using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FinanceDbContext _dbContext;
        public UserRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetById(Guid id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public Task<User?> GetByLogin(string login)
        {
            return _dbContext.Users.FirstOrDefaultAsync(user =>user.Login == login);
        }

        public async Task Update(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
