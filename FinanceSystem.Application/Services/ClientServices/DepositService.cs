using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Services.ClientServices
{
    public class DepositService
    {
        private readonly IDepositRepository _depositRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ILogRepository _logRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public DepositService(IDepositRepository depositRepository, ILogRepository logRepository, IBankRepository bankRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _depositRepository = depositRepository;
            _logRepository = logRepository;
            _bankRepository = bankRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public Deposit CreateDeposit(Guid clientId, Guid accountId, decimal amount, double rate, DepositType type)
        {
            var account = _accountRepository.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new UnauthorizedAccessException("This account does not belong to you");

            account.DecreaseBalance(amount);
            var deposit = Deposit.CreateNew(clientId, account.BankId, amount, rate, type);

            _accountRepository.Update(account);
            _depositRepository.Add(deposit);

            var transaction = Transaction.CreateNew(accountId, deposit.Id, amount,TransactionType.TransferAccountToDeposit);

            _transactionRepository.Add(transaction);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Create, deposit.Id, LogTargetType.Deposit));
            return deposit;
        }

        public void CloseDeposit(Guid clientId, Guid depositId, Guid accountId)
        {
            var deposit = _depositRepository.GetById(depositId);
            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");
            if (deposit.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            var account = _accountRepository.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            var amount = deposit.Balance;
            deposit.Close();
            account.IncreaseBalance(amount);

            _depositRepository.Update(deposit);
            _accountRepository.Update(account);

            var transaction = Transaction.CreateNew(depositId, accountId, amount, TransactionType.TransferDepositToAccount);
            _transactionRepository.Add(transaction);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Close, depositId, LogTargetType.Deposit));
        }

        public void AccrueInterest(Guid clientId, Guid depositId)
        {
            var deposit = _depositRepository.GetById(depositId);

            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");

            if (deposit.ClientId != clientId)
                throw new UnauthorizedAccessException("This deposit does not belong to you");
            deposit.AccrueInterest();
            _depositRepository.Update(deposit);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.AccrueInterest, depositId, LogTargetType.Deposit));
        }
    }
}
