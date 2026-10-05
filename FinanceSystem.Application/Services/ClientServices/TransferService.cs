using FinanceSystem.Application.Exceptions;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Interfaces;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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

        public async Task TransferAccountToAccount(Guid clientId, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var from = await _unitOfWork.Accounts.GetByNumberAsync(fromAccountNumber);
            var to = await _unitOfWork.Accounts.GetByNumberAsync(toAccountNumber);

            if (from == null || to == null)
                throw new NotFoundException("Account not found");

            if (from.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            if (from.Id == to.Id)
                throw new InvalidOperationException("Cannot transfer to the same account");

            from.DecreaseBalance(amount);
            to.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew(from.Id, to.Id ,amount, TransactionType.TransferAccountToAccount );
            await _unitOfWork.Transactions.AddAsync(transaction);

            await _unitOfWork.Logs.AddAsync(SystemLog.CreateTransferLog(clientId, ActionType.Transfer, transaction.Id, LogTargetType.Transaction,$"Account {from.Number}", $"Account {to.Number}", amount));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TransferAccountToDeposit(Guid clientId, string accountNumber, string depositNumber, decimal amount)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var account = await _unitOfWork.Accounts.GetByNumberAsync(accountNumber);
            var deposit = await _unitOfWork.Deposits.GetByNumberAsync(depositNumber);

            if (account == null || deposit == null)
                throw new NotFoundException("Entity not found");

            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            account.DecreaseBalance(amount);
            deposit.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew( account.Id, deposit.Id, amount, TransactionType.TransferAccountToDeposit);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.CreateTransferLog(clientId, ActionType.Transfer, transaction.Id, LogTargetType.Transaction, $"Account {account.Number}", $"Deposit {deposit.Number}", amount));


            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TransferDepositToAccount(Guid clientId, string depositNumber, string accountNumber, decimal amount)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var deposit = await _unitOfWork.Deposits.GetByNumberAsync(depositNumber);
            var account = await _unitOfWork.Accounts.GetByNumberAsync(accountNumber);

            if (deposit == null || account == null)
                throw new NotFoundException("Entity not found");

            if (deposit.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            deposit.DecreaseBalance(amount);
            account.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew(deposit.Id, account.Id, amount, TransactionType.TransferDepositToAccount);
            await _unitOfWork.Transactions.AddAsync(transaction);

            await _unitOfWork.Logs.AddAsync(SystemLog.CreateTransferLog(clientId, ActionType.Transfer, transaction.Id, LogTargetType.Transaction,  $"Deposit {deposit.Number}", $"Account {account.Number}", amount));

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TransferDepositToDeposit(Guid clientId, string fromDepositNumber, string toDepositNumber, decimal amount)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var from = await _unitOfWork.Deposits.GetByNumberAsync(fromDepositNumber);
            var to = await _unitOfWork.Deposits.GetByNumberAsync(toDepositNumber);

            if (from == null || to == null)
                throw new NotFoundException("Deposit not found");

            if (from.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            if (from.Id == to.Id)
                throw new InvalidOperationException("Cannot transfer to the same account");

            from.DecreaseBalance(amount);
            to.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew(from.Id, to.Id, amount, TransactionType.TransferDepositToDeposit);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.CreateTransferLog(clientId, ActionType.Transfer, transaction.Id, LogTargetType.Transaction, $"Deposit {from.Number}", $"Deposit {to.Number}", amount));

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
