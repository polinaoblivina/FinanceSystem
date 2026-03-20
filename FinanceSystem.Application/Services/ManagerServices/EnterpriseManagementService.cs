using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
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
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public EnterpriseManagementService(IEnterpriseRepository enterpriseRepository, IEmploymentRepository employmentRepository, IUserRepository userRepository, ILogRepository logRepository)
        {
            _enterpriseRepository = enterpriseRepository;
            _employmentRepository = employmentRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }
        public void AddClientToEnterprise(Guid managerId, Guid clientId, Guid enterpriseId)
        {
            var user = _userRepository.GetById(clientId);
            if (user == null)
                throw new InvalidOperationException("User not found");
            if (user is not Client)
                throw new InvalidOperationException("User is not a client");

            var enterprise = _enterpriseRepository.GetById(enterpriseId);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var existingEmployment = _employmentRepository.GetByClientAndEnterprise(clientId, enterpriseId);
            if (existingEmployment != null)
                throw new InvalidOperationException("Client is already added to this enterprise");

            var employment = Employment.CreateNew(clientId, enterpriseId);
            _employmentRepository.Add(employment);

            _logRepository.Add(SystemLog.Create(managerId, ActionType.Create,employment.Id, LogTargetType.Employment));
        }

        public void RemoveClientFromEnterprise(Guid managerId, Guid clientId, Guid enterpriseId)
        {
            var user = _userRepository.GetById(clientId);
            if (user == null)
                throw new InvalidOperationException("User not found");
            if (user is not Client)
                throw new InvalidOperationException("User is not a client");

            var enterprise = _enterpriseRepository.GetById(enterpriseId);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var employment = _employmentRepository.GetByClientAndEnterprise(clientId, enterpriseId);
            if (employment == null)
                throw new InvalidOperationException("Client is not employed at this enterprise");

            _employmentRepository.Remove(employment.Id);
            _logRepository.Add(SystemLog.Create(managerId, ActionType.Delete, employment.Id, LogTargetType.Employment));
        }
        public List<(Enterprise Enterprise, List<Client> Employees)> GetAllEnterprisesWithEmployees()
        {
            var enterprises = _enterpriseRepository.GetAll();
            if (enterprises == null)
                enterprises = new List<Enterprise>();
            var result = new List<(Enterprise, List<Client>)>();

            foreach (var enterprise in enterprises)
            {
                var employments = _employmentRepository.GetByEnterpriseId(enterprise.Id);
                var employees = new List<Client>();

                foreach (var employment in employments)
                {
                    var user = _userRepository.GetById(employment.ClientId);
                    if (user is Client client)
                        employees.Add(client);
                }
                result.Add((enterprise, employees));
            }

            return result;
        }
    }
}
