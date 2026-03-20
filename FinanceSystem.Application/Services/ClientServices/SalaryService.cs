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
    public class SalaryService
    {
        private readonly ISalaryProjectRequestRepository _requestRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly ILogRepository _logRepository;

        public SalaryService(ISalaryProjectRequestRepository requestRepository, ITransactionRepository transactionRepository, IAccountRepository accountRepository, IEnterpriseRepository enterpriseRepository, ILogRepository logRepository)
        {
            _requestRepository = requestRepository;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _enterpriseRepository = enterpriseRepository;
            _logRepository = logRepository;
        }
        public void RequestSalaryProject(Guid clientId, Guid enterpriseId)
        {
            var enterprise = _enterpriseRepository.GetById(enterpriseId);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var existing = _requestRepository.GetByClientAndEnterprise(clientId, enterpriseId);
            if (existing != null)
                throw new InvalidOperationException("Request already exists");

            var request = SalaryProjectRequest.CreateNew(clientId, enterpriseId);

            _requestRepository.Add(request);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.Create, request.Id, LogTargetType.SalaryProjectRequest));
        }

        public void ReceiveSalary(Guid clientId, Guid enterpriseId, Guid accountId, decimal amount)
        {
            var account = _accountRepository.GetById(accountId);

            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            var enterprise = _enterpriseRepository.GetById(enterpriseId);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var request = _requestRepository.GetApproved(clientId, enterpriseId);

            if (request == null)
                throw new InvalidOperationException("Salary project not approved");

            account.IncreaseBalance(amount);
            _accountRepository.Update(account);

            var transaction = Transaction.CreateNew(enterpriseId, accountId, amount, TransactionType.SalaryPayment);

            _transactionRepository.Add(transaction);

            _logRepository.Add(SystemLog.Create(clientId, ActionType.ReceiveSalary, accountId, LogTargetType.Account));
        }
    }
}
