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
    public class SalaryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalaryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task RequestSalaryProject(Guid clientId, Guid enterpriseId)
        {
            var enterprise = await _unitOfWork.Enterprises.GetByIdAsync(enterpriseId);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var existing = await _unitOfWork.SalaryRequests.GetByClientAndEnterpriseAsync(clientId, enterpriseId);
            if (existing != null)
                throw new InvalidOperationException("Request already exists");

            var request = SalaryProjectRequest.CreateNew(clientId, enterpriseId);

            await _unitOfWork.SalaryRequests.AddAsync(request);

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Create, request.Id, LogTargetType.SalaryProjectRequest));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReceiveSalary(Guid clientId, Guid enterpriseId, Guid accountId, decimal amount)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);

            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            var enterprise = await _unitOfWork.Enterprises.GetByIdAsync(enterpriseId);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var request = await _unitOfWork.SalaryRequests.GetByClientAndEnterpriseAsync(clientId, enterpriseId);

            if (request == null)
                throw new InvalidOperationException("No salary project request found");

            if (request.Status != RequestStatus.Accepted)
                throw new InvalidOperationException("Salary project not accepted");


            account.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew(enterpriseId, accountId, amount, TransactionType.SalaryPayment);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.ReceiveSalary, accountId, LogTargetType.Account));

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
