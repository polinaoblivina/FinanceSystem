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

        public Client Register(string login, string password, string firstName, string lastName)
            => _registrationService.Register(login, password, firstName, lastName);

        public List<Bank> GetBanks()
            => _infoService.GetBanks();
        public List<Enterprise> GetEnterprises()
            => _infoService.GetEnterprises();

        public Account OpenAccount(Guid clientId, Guid bankId)
            => _accountService.OpenAccount(clientId, bankId);
        public void CloseAccount(Guid clientId, Guid accountId)
            => _accountService.CloseAccount(clientId, accountId);
        public List<Transaction> GetAccountTransactions(Guid clientId, Guid accountId)
            => _accountService.GetTransactions(clientId, accountId);

        public Deposit CreateDeposit(Guid clientId, Guid bankId, decimal amount, double rate, DepositType type)
            => _depositService.CreateDeposit(clientId, bankId, amount, rate, type);
        public void CloseDeposit(Guid clientId, Guid depositId, Guid accountId)
            => _depositService.CloseDeposit(clientId, depositId, accountId);
        public void AccrueInterest(Guid clientId, Guid depositId)
            => _depositService.AccrueInterest(clientId, depositId);


        public void TransferAccountToAccount(Guid clientId, Guid fromAccountId, Guid toAccountId, decimal amount)
            => _transferService.TransferAccountToAccount(clientId, fromAccountId, toAccountId, amount);
        public void TransferAccountToDeposit(Guid clientId, Guid accountId, Guid depositId, decimal amount)
            => _transferService.TransferAccountToDeposit(clientId, accountId, depositId, amount);
        public void TransferDepositToAccount(Guid clientId, Guid depositId, Guid accountId, decimal amount)
            => _transferService.TransferDepositToAccount(clientId, depositId, accountId, amount);
        public void TransferDepositToDeposit(Guid clientId, Guid fromDepositId, Guid toDepositId, decimal amount)
            => _transferService.TransferDepositToDeposit(clientId, fromDepositId, toDepositId, amount);


        public void RequestSalaryProject(Guid clientId, Guid enterpriseId)
            => _salaryService.RequestSalaryProject(clientId, enterpriseId);
        public void ReceiveSalary(Guid clientId, Guid enterpriseId, Guid accountId, decimal amount)
            => _salaryService.ReceiveSalary(clientId, enterpriseId, accountId, amount);
    }
}