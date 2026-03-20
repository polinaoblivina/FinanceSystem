using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Services.AdminServices
{
    public class LogViewerService
    {
        private readonly ILogRepository _logRepository;
        public LogViewerService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public List<SystemLog> GetAllLogs() => _logRepository.GetAll();
    }
}
