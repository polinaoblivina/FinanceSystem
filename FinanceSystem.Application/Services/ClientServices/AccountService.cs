using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Repositories;
using FInanceSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Services.ClientServices
{
    public class AccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Account> OpenAccount(Guid clientId, Guid bankId)
        {
            var bank = await _unitOfWork.Banks.GetByIdAsync(bankId);
            if (bank == null)
                throw new InvalidOperationException("Bank not found");

            var account = Account.CreateNew(clientId, bankId);
            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Create, account.Id, LogTargetType.Account));

            await _unitOfWork.SaveChangesAsync();
            return account;
        }

        public async Task CloseAccount(Guid clientId, Guid accountId)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            account.Close();

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Close, accountId, LogTargetType.Account));
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<Transaction>> GetTransactions(Guid clientId, Guid accountId)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            return await _unitOfWork.Transactions.GetByAccountIdAsync(accountId);
        }
    }
}
