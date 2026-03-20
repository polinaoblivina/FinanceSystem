using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Interfaces;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Services.ManagerServices
{
    public class AccountControlService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AccountControlService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task BlockAccount(Guid managerId, Guid accountId)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            account.Block();

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Block, accountId, LogTargetType.Account));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnblockAccount(Guid managerId, Guid accountId)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            account.Unblock();
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Unblock, accountId, LogTargetType.Account));

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BlockDeposit(Guid managerId, Guid depositId)
        {
            var deposit = await _unitOfWork.Deposits.GetByIdAsync(depositId);
            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");

            deposit.Block();

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Block, depositId, LogTargetType.Deposit));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnblockDeposit(Guid managerId, Guid depositId)
        {
            var deposit = await _unitOfWork.Deposits.GetByIdAsync(depositId);
            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");

            deposit.Unblock();
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Unblock, depositId, LogTargetType.Deposit));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAccountTransactions(Guid accountId)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            return await _unitOfWork.Transactions.GetByAccountIdAsync(accountId);
        }
    }
}
