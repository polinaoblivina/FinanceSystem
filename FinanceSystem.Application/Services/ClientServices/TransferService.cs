using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public TransferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task TransferAccountToAccount(Guid clientId, Guid fromAccountId, Guid toAccountId, decimal amount)
        {

            var from = await _unitOfWork.Accounts.GetByIdAsync(fromAccountId);
            var to = await _unitOfWork.Accounts.GetByIdAsync(toAccountId);

            if (from == null || to == null)
                throw new InvalidOperationException("Account not found");

            if (from.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            if (fromAccountId == toAccountId)
                throw new InvalidOperationException("Cannot transfer to the same account");

            from.DecreaseBalance(amount);
            to.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew(fromAccountId, toAccountId,amount, TransactionType.TransferAccountToAccount );
            await _unitOfWork.Transactions.AddAsync(transaction);

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Transfer, fromAccountId, LogTargetType.Account));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TransferAccountToDeposit(Guid clientId, Guid accountId, Guid depositId, decimal amount)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            var deposit = await _unitOfWork.Deposits.GetByIdAsync(depositId);

            if (account == null || deposit == null)
                throw new InvalidOperationException("Entity not found");

            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            account.DecreaseBalance(amount);
            deposit.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew( accountId, depositId, amount, TransactionType.TransferAccountToDeposit);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Transfer, accountId, LogTargetType.Account));

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TransferDepositToAccount(Guid clientId, Guid depositId, Guid accountId, decimal amount)
        {
            var deposit = await _unitOfWork.Deposits.GetByIdAsync(depositId);
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);

            if (deposit == null || account == null)
                throw new InvalidOperationException("Entity not found");

            if (deposit.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            deposit.DecreaseBalance(amount);
            account.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew(depositId, accountId, amount, TransactionType.TransferDepositToAccount);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Transfer, depositId, LogTargetType.Deposit));

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TransferDepositToDeposit(Guid clientId, Guid fromDepositId, Guid toDepositId, decimal amount)
        {
            var from = await _unitOfWork.Deposits.GetByIdAsync(fromDepositId);
            var to = await _unitOfWork.Deposits.GetByIdAsync(toDepositId);

            if (from == null || to == null)
                throw new InvalidOperationException("Deposit not found");

            if (from.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            if (fromDepositId == toDepositId)
                throw new InvalidOperationException("Cannot transfer to the same account");

            from.DecreaseBalance(amount);
            to.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew(fromDepositId, toDepositId, amount, TransactionType.TransferDepositToDeposit);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Transfer, fromDepositId, LogTargetType.Deposit));

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
