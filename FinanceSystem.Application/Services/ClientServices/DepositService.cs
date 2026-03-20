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

        public async Task<Deposit> CreateDeposit(Guid clientId, Guid accountId, decimal amount, double rate, DepositType type)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new UnauthorizedAccessException("This account does not belong to you");

            account.DecreaseBalance(amount);
            var deposit = Deposit.CreateNew(clientId, account.BankId, amount, rate, type);

            await _unitOfWork.Deposits.AddAsync(deposit);

            var transaction = Transaction.CreateNew(accountId, deposit.Id, amount,TransactionType.TransferAccountToDeposit);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Create, deposit.Id, LogTargetType.Deposit));

            await _unitOfWork.SaveChangesAsync();
            return deposit;
        }

        public async Task CloseDeposit(Guid clientId, Guid depositId, Guid accountId)
        {
            var deposit = await _unitOfWork.Deposits.GetByIdAsync(depositId);
            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");
            if (deposit.ClientId != clientId)
                throw new InvalidOperationException("Not your deposit");

            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            var amount = deposit.Balance;
            deposit.Close();
            account.IncreaseBalance(amount);


            var transaction = Transaction.CreateNew(depositId, accountId, amount, TransactionType.TransferDepositToAccount);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Close, depositId, LogTargetType.Deposit));

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AccrueInterest(Guid clientId, Guid depositId)
        {
            var deposit = await _unitOfWork.Deposits.GetByIdAsync(depositId);

            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");

            if (deposit.ClientId != clientId)
                throw new UnauthorizedAccessException("This deposit does not belong to you");

            deposit.AccrueInterest();

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.AccrueInterest, depositId, LogTargetType.Deposit));
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
