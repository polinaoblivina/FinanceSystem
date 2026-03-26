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
        public async Task BlockAccount(Guid managerId, string accountNumber)
        {
            var account = await _unitOfWork.Accounts.GetByNumberAsync(accountNumber);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            account.Block();

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Block, account.Id, LogTargetType.Account));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnblockAccount(Guid managerId, string accountNumber)
        {
            var account = await _unitOfWork.Accounts.GetByNumberAsync(accountNumber);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            account.Unblock();
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Unblock, account.Id, LogTargetType.Account));

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BlockDeposit(Guid managerId, string depositNumber)
        {
            var deposit = await _unitOfWork.Deposits.GetByNumberAsync(depositNumber);
            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");

            deposit.Block();

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Block, deposit.Id, LogTargetType.Deposit));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnblockDeposit(Guid managerId, string depositNumber)
        {
            var deposit = await _unitOfWork.Deposits.GetByNumberAsync(depositNumber);
            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");

            deposit.Unblock();
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Unblock, deposit.Id, LogTargetType.Deposit));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAccountTransactions(string accountNumber)
        {
            var account = await _unitOfWork.Accounts.GetByNumberAsync(accountNumber);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            return await _unitOfWork.Transactions.GetByAccountIdAsync(account.Id);
        }

        public async Task<List<Account>> GetClientAccounts(string clientLogin)
        {
            var user = await _unitOfWork.Users.GetByLoginAsync(clientLogin);
            if (user == null)
                throw new InvalidOperationException($"Client not found");
            if (user is not Client client)
                throw new InvalidOperationException($"User  is not a client");

            return await _unitOfWork.Accounts.GetByClientIdAsync(client.Id);
        }

        public async Task<List<Deposit>> GetClientDeposits(string clientLogin)
        {
            var user = await _unitOfWork.Users.GetByLoginAsync(clientLogin);
            if (user == null)
                throw new InvalidOperationException($"Client not found");
            if (user is not Client client)
                throw new InvalidOperationException($"User is not a client");

            return await _unitOfWork.Deposits.GetByClientIdAsync(client.Id);
        }
    }
}
