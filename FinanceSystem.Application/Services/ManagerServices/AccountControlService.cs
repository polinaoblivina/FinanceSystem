using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
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
        private readonly IAccountRepository _accountRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogRepository _logRepository;

        public AccountControlService(IAccountRepository accountRepository, IDepositRepository depositRepository, ITransactionRepository transactionRepository, ILogRepository logRepository)
        {
            _accountRepository = accountRepository;
            _depositRepository = depositRepository;
            _transactionRepository = transactionRepository;
            _logRepository = logRepository;
        }
        public void BlockAccount(Guid managerId, Guid accountId)
        {
            var account = _accountRepository.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            account.Block();
            _accountRepository.Update(account);

            _logRepository.Add(SystemLog.Create(managerId, ActionType.Block, accountId, LogTargetType.Account));
        }

        public void UnblockAccount(Guid managerId, Guid accountId)
        {
            var account = _accountRepository.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            account.Unblock();
            _accountRepository.Update(account);

            _logRepository.Add(SystemLog.Create(managerId, ActionType.Unblock, accountId, LogTargetType.Account));
        }

        public void BlockDeposit(Guid managerId, Guid depositId)
        {
            var deposit = _depositRepository.GetById(depositId);
            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");

            deposit.Block();
            _depositRepository.Update(deposit);

            _logRepository.Add(SystemLog.Create(managerId, ActionType.Block, depositId, LogTargetType.Deposit));
        }

        public void UnblockDeposit(Guid managerId, Guid depositId)
        {
            var deposit = _depositRepository.GetById(depositId);
            if (deposit == null)
                throw new InvalidOperationException("Deposit not found");

            deposit.Unblock();
            _depositRepository.Update(deposit);

            _logRepository.Add(SystemLog.Create(managerId, ActionType.Unblock, depositId, LogTargetType.Deposit));
        }

        public List<Transaction> GetAccountTransactions(Guid accountId)
        {
            var account = _accountRepository.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            return _transactionRepository.GetByTarget(accountId);
        }
    }
}
