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

        public async Task ApproveClient(Guid managerId, Guid clientId)
           => await _clientService.ApproveClient(managerId, clientId);
        public async Task GetAllClients()
            => await _clientService.GetAllClients();
        public async Task AddClientToEnterprise(Guid managerId, Guid clientId, Guid enterpriseId)
            => await _enterpriseService.AddClientToEnterprise(managerId, clientId, enterpriseId);
        public async Task RemoveClientFromEnterprise(Guid managerId, Guid clientId, Guid enterpriseId)
            => await _enterpriseService.RemoveClientFromEnterprise(managerId, clientId, enterpriseId);
        public async Task<List<(Enterprise Enterprise, List<Client> Employees)>> GetAllEnterprisesWithEmployees()
            => await _enterpriseService.GetAllEnterprisesWithEmployees();
        public async Task ApproveSalaryRequest(Guid managerId, Guid requestId)
            => await _salaryService.ApproveSalaryRequest(managerId, requestId);
        public async Task RejectSalaryRequest(Guid managerId, Guid requestId) 
            => await _salaryService.RejectSalaryRequest(managerId, requestId);
        public async Task BlockAccount(Guid managerId, Guid accountId)
            => await _accountService.BlockAccount(managerId, accountId);
        public async Task UnblockAccount(Guid managerId, Guid accountId)
            => await _accountService.UnblockAccount(managerId, accountId);
        public async Task BlockDeposit(Guid managerId, Guid depositId)
            => await _accountService.BlockDeposit(managerId, depositId);
        public async Task UnblockDeposit(Guid managerId, Guid depositId)
            => await _accountService.UnblockDeposit(managerId, depositId);
        public async Task<List<Transaction>> GetAccountTransactions(Guid accountId)
            => await _accountService.GetAccountTransactions(accountId);
    }
}


