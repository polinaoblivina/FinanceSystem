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
    public class TransferService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogRepository _logRepository;

        public TransferService(IAccountRepository accountRepository, IDepositRepository depositRepository, ITransactionRepository transactionRepository, ILogRepository logRepository)
        {
            _accountRepository = accountRepository;
            _depositRepository = depositRepository;
            _transactionRepository = transactionRepository;
            _logRepository = logRepository;
        }

        public void TransferAccountToAccount(Guid clientId, Guid fromAccountId, Guid toAccountId, decimal amount)
        {

            var from = _accountRepository.GetById(fromAccountId);
            var to = _accountRepository.GetById(toAccountId);

            if (from == null || to == null)
                throw new InvalidOperationException("Account not found");

            if (from.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            if (fromAccountId == toAccountId)
                throw new InvalidOperationException("Cannot transfer to the same account");

            from.DecreaseBalance(amount);
            to.IncreaseBalance(amount);

            _accountRepository.Update(from);
            _accountRepository.Update(to);

            var transaction = Transaction.CreateNew(fromAccountId, toAccountId,amount, TransactionType.TransferAccountToAccount );
            _transactionRepository.Add(transaction);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Transfer, fromAccountId, LogTargetType.Account));
        }

        public void TransferAccountToDeposit(Guid clientId, Guid accountId, Guid depositId, decimal amount)
        {
            var account = _accountRepository.GetById(accountId);
            var deposit = _depositRepository.GetById(depositId);

            if (account == null || deposit == null)
                throw new InvalidOperationException("Entity not found");

            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            account.DecreaseBalance(amount);
            deposit.IncreaseBalance(amount);

            _accountRepository.Update(account);
            _depositRepository.Update(deposit);

            var transaction = Transaction.CreateNew( accountId, depositId, amount, TransactionType.TransferAccountToDeposit);
            _transactionRepository.Add(transaction);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Transfer, accountId, LogTargetType.Account));
        }

        public void TransferDepositToAccount(Guid clientId, Guid depositId, Guid accountId, decimal amount)
        {
            var deposit = _depositRepository.GetById(depositId);
            var account = _accountRepository.GetById(accountId);

            if (deposit == null || account == null)
                throw new InvalidOperationException("Entity not found");

            if (deposit.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            deposit.DecreaseBalance(amount);
            account.IncreaseBalance(amount);

            _depositRepository.Update(deposit);
            _accountRepository.Update(account);

            var transaction = Transaction.CreateNew(depositId, accountId, amount, TransactionType.TransferDepositToAccount);
            _transactionRepository.Add(transaction);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Transfer, depositId, LogTargetType.Deposit));
        }

        public void TransferDepositToDeposit(Guid clientId, Guid fromDepositId, Guid toDepositId, decimal amount)
        {
            var from = _depositRepository.GetById(fromDepositId);
            var to = _depositRepository.GetById(toDepositId);

            if (from == null || to == null)
                throw new InvalidOperationException("Deposit not found");

            if (from.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            if (fromDepositId == toDepositId)
                throw new InvalidOperationException("Cannot transfer to the same account");

            from.DecreaseBalance(amount);
            to.IncreaseBalance(amount);

            _depositRepository.Update(from);
            _depositRepository.Update(to);

            var transaction = Transaction.CreateNew(fromDepositId, toDepositId, amount, TransactionType.TransferDepositToDeposit);
            _transactionRepository.Add(transaction);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Transfer, fromDepositId, LogTargetType.Deposit));
        }
    }
}
