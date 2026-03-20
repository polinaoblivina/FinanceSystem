using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Services.AdminServices
{
    public class ActionRevertService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISalaryProjectRequestRepository _requestRepository;
        private readonly ILogRepository _logRepository;

        public ActionRevertService(IAccountRepository accountRepository, IDepositRepository depositRepository, ITransactionRepository transactionRepository, IUserRepository userRepository, ISalaryProjectRequestRepository requestRepository, ILogRepository logRepository)
        {
            _accountRepository = accountRepository;
            _depositRepository = depositRepository;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _logRepository = logRepository;
        }

        public void RevertByLogId(Guid adminId, Guid logId)
        {
            var log = _logRepository.GetById(logId);
            if (log == null)
                throw new InvalidOperationException($"Log with id {logId} not found");

            if (log.Action == ActionType.Login || log.Action == ActionType.Logout)
                throw new InvalidOperationException("Cannot revert login/logout actions");
        }
    }
}
