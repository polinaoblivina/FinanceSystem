using FinanceSystem.Application.Interfaces;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public SeedService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAllData()
        {
            await SeedBanks();
            await SeedEnterprises();
            await SeedUsers();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SeedBanks()
        {
            var existingBanks = await _unitOfWork.Banks.GetAllAsync();
            if (existingBanks.Count == 0)
            {
                var bank1 = Bank.CreateNew("Bank1");
                var bank2 = Bank.CreateNew("Bank2");
                await _unitOfWork.Banks.AddAsync(bank1);
                await _unitOfWork.Banks.AddAsync(bank2);
            }
        }
        public async Task SeedEnterprises()
        {
            var existingEnterprises = await _unitOfWork.Enterprises.GetAllAsync();
            if (existingEnterprises.Count == 0)
            {
                var enterprise1 = Enterprise.CreateNew("Enterprise1");
                var enterprise2 = Enterprise.CreateNew("Enterprise2");
                await _unitOfWork.Enterprises.AddAsync(enterprise1);
                await _unitOfWork.Enterprises.AddAsync(enterprise2);
            }
        }

        public async Task SeedUsers()
        {
            var existingUsers = await _unitOfWork.Users.GetAllAsync();
            if (existingUsers.Count == 0)
            {
                var admin = Admin.CreateNew("admin", _passwordHasher.Hash("adminparol"), "Admin", "Adminovich");
                await _unitOfWork.Users.AddAsync(admin);

                var manager = Manager.CreateNew("manager", _passwordHasher.Hash("managerparol"), "Manager", "Managerovich");
                await _unitOfWork.Users.AddAsync(manager);
            }
        }
    }
}
