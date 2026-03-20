using FInanceSystem.Domain.Entities;
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
        private readonly IBankRepository _bankRepository;
        private readonly IEnterpriseRepository _enterpriseRepository;

        public InfoService(IBankRepository bankRepository, IEnterpriseRepository enterpriseRepository)
        {
            _bankRepository = bankRepository;
            _enterpriseRepository = enterpriseRepository;
        }

        public List<Bank> GetBanks()
            => _bankRepository.GetAll();

        public List<Enterprise> GetEnterprises()
            => _enterpriseRepository.GetAll();
    }
}
