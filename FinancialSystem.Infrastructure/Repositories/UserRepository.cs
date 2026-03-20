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
        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users
                            .FindAsync(id);
        }
        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(user => user.Login == login);
        }
        public Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
