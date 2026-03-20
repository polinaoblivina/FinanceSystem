using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Interfaces;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Services.ClientServices
{
    public class InfoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Bank>> GetBanks()
            => await _unitOfWork.Banks.GetAllAsync();

        public async Task<List<Enterprise>> GetEnterprises()
            => await _unitOfWork.Enterprises.GetAllAsync();
    }
}
