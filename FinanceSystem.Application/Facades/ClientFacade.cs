using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.Common;
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
    public class ClientFacade
    {
        private readonly ClientRegistrationService _registrationService;
        private readonly AccountService _accountService;
        private readonly DepositService _depositService;
        private readonly TransferService _transferService;
        private readonly InfoService _infoService;
        private readonly SalaryService _salaryService;

        public ClientFacade(
            ClientRegistrationService registrationService,
            AccountService accountService,
            DepositService depositService,
            TransferService transferService,
            InfoService infoService,
            SalaryService salaryService)
        {
            _registrationService = registrationService;
            _accountService = accountService;
            _depositService = depositService;
            _transferService = transferService;
            _infoService = infoService;
            _salaryService = salaryService;
        }

        public async Task<Client> Register(string login, string password, string firstName, string lastName)
            => await _registrationService.Register(login, password, firstName, lastName);

        public async Task<List<Bank>> GetBanks()
            => await _infoService.GetBanks();
        public async Task<List<Enterprise>> GetEnterprises()
            => await _infoService.GetEnterprises();

        public async Task<Account> OpenAccount(Guid clientId, Guid bankId)
            => await _accountService.OpenAccount(clientId, bankId);
        public async Task CloseAccount(Guid clientId, Guid accountId)
            => await _accountService.CloseAccount(clientId, accountId);
        public async Task<List<Transaction>> GetAccountTransactions(Guid clientId, Guid accountId)
            => await _accountService.GetTransactions(clientId, accountId);

        public async Task<Deposit> CreateDeposit(Guid clientId, Guid bankId, decimal amount, double rate, DepositType type)
            => await _depositService.CreateDeposit(clientId, bankId, amount, rate, type);
        public async Task CloseDeposit(Guid clientId, Guid depositId, Guid accountId)
            =>  await _depositService.CloseDeposit(clientId, depositId, accountId);
        public async Task AccrueInterest(Guid clientId, Guid depositId)
            => await  _depositService.AccrueInterest(clientId, depositId);


        public async Task TransferAccountToAccount(Guid clientId, Guid fromAccountId, Guid toAccountId, decimal amount)
            => await _transferService.TransferAccountToAccount(clientId, fromAccountId, toAccountId, amount);
        public async Task TransferAccountToDeposit(Guid clientId, Guid accountId, Guid depositId, decimal amount)
            => await _transferService.TransferAccountToDeposit(clientId, accountId, depositId, amount);
        public async Task TransferDepositToAccount(Guid clientId, Guid depositId, Guid accountId, decimal amount)
            => await _transferService.TransferDepositToAccount(clientId, depositId, accountId, amount);
        public async Task TransferDepositToDeposit(Guid clientId, Guid fromDepositId, Guid toDepositId, decimal amount)
            => await _transferService.TransferDepositToDeposit(clientId, fromDepositId, toDepositId, amount);


        public async Task RequestSalaryProject(Guid clientId, Guid enterpriseId)
            => await _salaryService.RequestSalaryProject(clientId, enterpriseId);
        public async Task ReceiveSalary(Guid clientId, Guid enterpriseId, Guid accountId, decimal amount)
            => await _salaryService.ReceiveSalary(clientId, enterpriseId, accountId, amount);
    }
}