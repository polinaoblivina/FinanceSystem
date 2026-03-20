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
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ILogRepository _logRepository;

        public AccountService(IAccountRepository accountRepository, ITransactionRepository transactionRepository, ILogRepository logRepository, IBankRepository bankRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _logRepository = logRepository;
            _bankRepository = bankRepository;
        }
        public Account OpenAccount(Guid clientId, Guid bankId)
        {
            var bank = _bankRepository.GetById(bankId);
            if (bank == null)
                throw new InvalidOperationException("Bank not found");

            var account = Account.CreateNew(clientId, bankId);
            _accountRepository.Add(account);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Create, account.Id, LogTargetType.Account));

            return account;
        }

        public void CloseAccount(Guid clientId, Guid accountId)
        {
            var account = _accountRepository.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            account.Close();
            _accountRepository.Update(account);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Close, accountId, LogTargetType.Account));
        }
        public List<Transaction> GetTransactions(Guid clientId, Guid accountId)
        {
            var account = _accountRepository.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            return _transactionRepository.GetByTarget(accountId);
        }
    }
}
