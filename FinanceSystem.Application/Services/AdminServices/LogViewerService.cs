using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public LogViewerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SystemLog>> GetAllLogs()
            => await _unitOfWork.Logs.GetAllAsync();
    }
}
