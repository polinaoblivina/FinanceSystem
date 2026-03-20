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
    public class LogRepository : ILogRepository
    {
        private readonly FinanceDbContext _dbContext;

        public LogRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(SystemLog log)
        {
            await _dbContext.Logs.AddAsync(log);
        }
        public async Task<List<SystemLog>> GetAllAsync()
        {
            return await _dbContext.Logs.ToListAsync();
        }
        public async Task<SystemLog?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Logs.FindAsync(id);
        }
    }
}
