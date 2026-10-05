using FinanceSystem.Application.Exceptions;
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
    public class DepositService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepositService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Deposit> CreateDeposit(Guid clientId, string number, decimal amount, double rate, DepositType type)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var account = await _unitOfWork.Accounts.GetByNumberAsync(number);
            if (account == null)
                throw new NotFoundException("Account not found");
            if (account.ClientId != clientId)
                throw new UnauthorizedAccessException("This account does not belong to you");

            account.DecreaseBalance(amount);
            var deposit = Deposit.CreateNew(clientId, account.BankId, amount, rate, type);

            await _unitOfWork.Deposits.AddAsync(deposit);

            var transaction = Transaction.CreateNew(account.Id, deposit.Id, amount,TransactionType.TransferAccountToDeposit);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Create, deposit.Id, LogTargetType.Deposit));

            await _unitOfWork.SaveChangesAsync();
            return deposit;
        }

        public async Task CloseDeposit(Guid clientId, string depositNumber, string accountNumber)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var deposit = await _unitOfWork.Deposits.GetByNumberAsync(depositNumber);
            if (deposit == null)
                throw new NotFoundException("Deposit not found");
            if (deposit.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            var account = await _unitOfWork.Accounts.GetByNumberAsync(accountNumber);
            if (account == null)
                throw new NotFoundException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            var amount = deposit.Balance;
            deposit.Close();
            account.IncreaseBalance(amount);


            var transaction = Transaction.CreateNew(deposit.Id, account.Id, amount, TransactionType.TransferDepositToAccount);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.CreateTransferLog(clientId, ActionType.Close, deposit.Id, LogTargetType.Deposit, $"Deposit {deposit.Number}", $"Account {account.Number}", amount));

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AccrueInterest(Guid clientId, string depositNumber)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var deposit = await _unitOfWork.Deposits.GetByNumberAsync(depositNumber);

            if (deposit == null)
                throw new NotFoundException("Deposit not found");

            if (deposit.ClientId != clientId)
                throw new UnauthorizedAccessException("This deposit does not belong to you");

            deposit.AccrueInterest();

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.AccrueInterest, deposit.Id, LogTargetType.Deposit));
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<Deposit>> GetClientDeposits(Guid clientId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            return await _unitOfWork.Deposits.GetByClientIdAsync(clientId);
        }
    }
}
