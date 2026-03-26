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
        public async Task RequestSalaryProject(Guid clientId, string enterpriseName)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var enterprise = await _unitOfWork.Enterprises.GetByNameAsync(enterpriseName);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var employment = await _unitOfWork.Employments.GetByClientAndEnterpriseAsync(clientId, enterprise.Id);
            if (employment == null)
                throw new InvalidOperationException($"You are not employed at {enterpriseName}.");

            var existing = await _unitOfWork.SalaryRequests.GetByClientAndEnterpriseAsync(clientId, enterprise.Id);
            if (existing != null)
                throw new InvalidOperationException("Request already exists");

            var request = SalaryProjectRequest.CreateNew(clientId, enterprise.Id);

            await _unitOfWork.SalaryRequests.AddAsync(request);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(clientId, ActionType.Create, request.Id, LogTargetType.SalaryProjectRequest));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReceiveSalary(Guid clientId, string enterpriseName, string accountNumber, decimal amount)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user is Client client && !client.IsApprovedByManager)
                throw new UnauthorizedAccessException("Your account is not approved by manager yet.");

            var account = await _unitOfWork.Accounts.GetByNumberAsync(accountNumber);

            if (account == null)
                throw new InvalidOperationException("Account not found");
            if (account.ClientId != clientId)
                throw new InvalidOperationException("Not your account");

            var enterprise = await _unitOfWork.Enterprises.GetByNameAsync(enterpriseName);
            if (enterprise == null)
                throw new InvalidOperationException("Enterprise not found");

            var employment = await _unitOfWork.Employments.GetByClientAndEnterpriseAsync(clientId, enterprise.Id);
            if (employment == null)
                throw new InvalidOperationException($"You are not employed at {enterpriseName}.");

            var request = await _unitOfWork.SalaryRequests.GetByClientAndEnterpriseAsync(clientId, enterprise.Id);

            if (request == null)
                throw new InvalidOperationException("No salary project request found");

            if (request.Status != RequestStatus.Accepted)
                throw new InvalidOperationException("Salary project not accepted");


            account.IncreaseBalance(amount);

            var transaction = Transaction.CreateNew(enterprise.Id, account.Id, amount, TransactionType.SalaryPayment);
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.Logs.AddAsync(SystemLog.CreateTransferLog(clientId,ActionType.ReceiveSalary,transaction.Id,LogTargetType.Transaction, $"Enterprise {enterprise.Name}", $"Account {account.Number}",amount));

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
