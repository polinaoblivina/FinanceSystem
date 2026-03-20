using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Services.Common
{
    public class SeedService
    {
        private readonly IBankRepository _bankRepository;
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly IUserRepository _userRepository;
        public SeedService(IBankRepository bankRepository,IEnterpriseRepository enterpriseRepository, IUserRepository userRepository)
        {
            _bankRepository = bankRepository;
            _enterpriseRepository = enterpriseRepository;
            _userRepository = userRepository;
        }

        public void SeedAllData()
        {
            SeedBanks();
            SeedEnterprises();
        }

        public void SeedBanks()
        {
            var bank1 = Bank.CreateNew("Bank1");
            var bank2 = Bank.CreateNew("Bank2");
            _bankRepository.Add(bank1);
            _bankRepository.Add(bank2);
        }
        public void SeedEnterprises()
        {
            var enterprise1 = Enterprise.CreateNew("Enterprise1");
            var enterprise2 = Enterprise.CreateNew("Enterprise2");
            _enterpriseRepository.Add(enterprise1);
            _enterpriseRepository.Add(enterprise2);
        }
    }
}
