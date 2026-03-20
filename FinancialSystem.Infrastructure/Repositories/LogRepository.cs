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
        public async Task Add(SystemLog log)
        {
            _dbContext.Logs.Add(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<SystemLog>> GetAll()
        {
            return await _dbContext.Logs.ToListAsync();
        }

        public async Task<SystemLog?> GetById(Guid id)
        {
            return await _dbContext.Logs.FirstOrDefaultAsync(log => log.Id == id);
        }
    }
}
