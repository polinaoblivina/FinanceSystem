using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Interfaces;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Services.ManagerServices
{
    public class EnterpriseManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EnterpriseManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddClientToEnterprise(Guid managerId, string clientLogin, string enterpriseName)
        {
            var client = await _unitOfWork.Users.GetByLoginAsync(clientLogin);
            if (client == null)
                throw new InvalidOperationException("User not found");
            if (client is not Client)
                throw new InvalidOperationException("User is not a client");

            var enterprise = await _unitOfWork.Enterprises.GetByNameAsync(enterpriseName);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var existingEmployment = await _unitOfWork.Employments.GetByClientAndEnterpriseAsync(client.Id, enterprise.Id);
            if (existingEmployment != null)
                throw new InvalidOperationException("Client is already added to this enterprise");

            var employment = Employment.CreateNew(client.Id, enterprise.Id);
            await _unitOfWork.Employments.AddAsync(employment);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Create, employment.Id, LogTargetType.Employment));

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveClientFromEnterprise(Guid managerId, string clientLogin, string enterpriseName)
        {
            var client = await _unitOfWork.Users.GetByLoginAsync(clientLogin);
            if (client == null)
                throw new InvalidOperationException("User not found");
            if (client is not Client)
                throw new InvalidOperationException("User is not a client");

            var enterprise = await _unitOfWork.Enterprises.GetByNameAsync(enterpriseName);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var employment = await _unitOfWork.Employments.GetByClientAndEnterpriseAsync(client.Id, enterprise.Id);
            if (employment == null)
                throw new InvalidOperationException("Client is not employed at this enterprise");

            await _unitOfWork.Employments.RemoveAsync(employment.Id);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Delete, employment.Id, LogTargetType.Employment));

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<(Enterprise, List<Client>)>> GetAllEnterprisesWithEmployees()
        {
            var enterprises = await _unitOfWork.Enterprises.GetAllAsync();
            if (enterprises == null)
                throw new InvalidOperationException("Enterprise not found");

            var result = new List<(Enterprise, List<Client>)>();

            foreach (var enterprise in enterprises)
            {
                var employments = await _unitOfWork.Employments.GetByEnterpriseIdAsync(enterprise.Id);
                var employees = new List<Client>();

                foreach (var employment in employments)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(employment.ClientId);
                    if (user is Client client)
                        employees.Add(client);
                }
                result.Add((enterprise, employees));
            }
            return result;
        }
    }
}
