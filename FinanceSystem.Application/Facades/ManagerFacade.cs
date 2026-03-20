using FinanceSystem.Application.Services.ManagerServices;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Facades
{
    public class ManagerFacade
    {
        private readonly ClientManagementService _clientService;
        private readonly EnterpriseManagementService _enterpriseService;
        private readonly SalaryRequestService _salaryService;
        private readonly AccountControlService _accountService;
        public ManagerFacade(ClientManagementService clientService, EnterpriseManagementService enterpriseService, SalaryRequestService salaryService, AccountControlService accountService)
        {
            _clientService = clientService;
            _enterpriseService = enterpriseService;
            _salaryService = salaryService;
            _accountService = accountService;
        }

        public void ApproveClient(Guid managerId, Guid clientId)
           => _clientService.ApproveClient(managerId, clientId);
        public void GetAllClients()
            => _clientService.GetAllClients();
        public void AddClientToEnterprise(Guid managerId, Guid clientId, Guid enterpriseId)
            => _enterpriseService.AddClientToEnterprise(managerId, clientId, enterpriseId);
        public void RemoveClientFromEnterprise(Guid managerId, Guid clientId, Guid enterpriseId)
            => _enterpriseService.RemoveClientFromEnterprise(managerId, clientId, enterpriseId);
        public List<(Enterprise Enterprise, List<Client> Employees)> GetAllEnterprisesWithEmployees()
            => _enterpriseService.GetAllEnterprisesWithEmployees();
        public void ApproveSalaryRequest(Guid managerId, Guid requestId)
            => _salaryService.ApproveSalaryRequest(managerId, requestId);
        public void RejectSalaryRequest(Guid managerId, Guid requestId) 
            => _salaryService.RejectSalaryRequest(managerId, requestId); 
        public void BlockAccount(Guid managerId, Guid accountId)
            => _accountService.BlockAccount(managerId, accountId);
        public void UnblockAccount(Guid managerId, Guid accountId)
            => _accountService.UnblockAccount(managerId, accountId);
        public void BlockDeposit(Guid managerId, Guid depositId)
            => _accountService.BlockDeposit(managerId, depositId);
        public void UnblockDeposit(Guid managerId, Guid depositId)
            => _accountService.UnblockDeposit(managerId, depositId);
        public List<Transaction> GetAccountTransactions(Guid accountId)
            => _accountService.GetAccountTransactions(accountId);
    }
}


