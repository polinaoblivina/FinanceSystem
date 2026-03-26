using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public ActionRevertService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RevertByLogId(Guid adminId, string logNumber)
        {
            var log = await _unitOfWork.Logs.GetByNumberAsync(logNumber);
            if (log == null)
                throw new InvalidOperationException($"Log with number {logNumber} not found");
            if (log.Action == ActionType.Revert)
                throw new InvalidOperationException("Cannot revert a revert action");

            switch (log.Action)
            {
                case ActionType.Create:
                    await RevertCreation(log);
                    break;
                case ActionType.Transfer:
                    await RevertTransfer(log);
                    break;
                case ActionType.Close:
                    await RevertClose(log);
                    break;
                case ActionType.Block:
                    await RevertBlock(log);
                    break;
                case ActionType.Unblock:
                    await RevertUnblock(log);
                    break;
                case ActionType.Approve:
                    await RevertApprove(log);
                    break;
                case ActionType.Reject:
                    await RevertReject(log);
                    break;
                case ActionType.ReceiveSalary:
                    await RevertReceiveSalary(log);
                    break;
                case ActionType.AccrueInterest:
                    throw new InvalidOperationException("Cannot revert interest accrual");
                default:
                    throw new InvalidOperationException($"Cannot revert action: {log.Action}");
            }

            var revertLog = SystemLog.CreateRevertLog(adminId, log.Number, log.Action.ToString());
            await _unitOfWork.Logs.AddAsync(revertLog);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task RevertCreation(SystemLog log)
        {
            if (!log.TargetId.HasValue || !log.TargetType.HasValue)
                throw new InvalidOperationException("Invalid target type or target id in log");

            switch (log.TargetType.Value)
            {
                case LogTargetType.Account:
                    var account = await _unitOfWork.Accounts.GetByIdAsync(log.TargetId.Value);
                    if (account == null)
                        throw new InvalidOperationException($"Account with id {log.TargetId.Value} not found");

                    account.Close();
                    await _unitOfWork.Accounts.UpdateAsync(account);
                    break;

                case LogTargetType.Deposit:
                    var deposit = await _unitOfWork.Deposits.GetByIdAsync(log.TargetId.Value);
                    if (deposit == null)
                        throw new InvalidOperationException($"Deposit with id {log.TargetId.Value} not found");

                    deposit.Close(false);
                    await _unitOfWork.Deposits.UpdateAsync(deposit);
                    break;

                case LogTargetType.SalaryProjectRequest:
                    var request = await _unitOfWork.SalaryRequests.GetByIdAsync(log.TargetId.Value);
                    if (request == null)
                        throw new InvalidOperationException($"Salary request with id {log.TargetId.Value} not found");

                    request.Reject();
                    await _unitOfWork.SalaryRequests.UpdateAsync(request);
                    break;

                default:
                    throw new InvalidOperationException($"Cannot revert creation for target type {log.TargetType.Value}");
            }
        }

        private async Task RevertTransfer(SystemLog log)
        {
            if (!log.TargetId.HasValue)
                throw new InvalidOperationException("Invalid target id in log");

            var transaction = await _unitOfWork.Transactions.GetByIdAsync(log.TargetId.Value);
            if (transaction == null)
                throw new InvalidOperationException($"Transaction with id {log.TargetId.Value} not found");

            if (transaction.FromId == Guid.Empty)
                throw new InvalidOperationException($"Transaction {transaction.Id} has empty FromId");

            if (transaction.ToId == Guid.Empty)
                throw new InvalidOperationException($"Transaction {transaction.Id} has empty ToId");

            var fromAccount = await _unitOfWork.Accounts.GetByIdAsync(transaction.FromId);
            if (fromAccount != null)
            {
                fromAccount.IncreaseBalance(transaction.Amount);
                await _unitOfWork.Accounts.UpdateAsync(fromAccount);
            }
            else
            {
                var fromDeposit = await _unitOfWork.Deposits.GetByIdAsync(transaction.FromId);
                if (fromDeposit != null)
                {
                    fromDeposit.IncreaseBalance(transaction.Amount);
                    await _unitOfWork.Deposits.UpdateAsync(fromDeposit);
                }
                else
                {
                    throw new InvalidOperationException($"Source entity with id {transaction.FromId} not found (neither account nor deposit)");
                }
            }

            var toAccount = await _unitOfWork.Accounts.GetByIdAsync(transaction.ToId);
            if (toAccount != null)
            {
                toAccount.DecreaseBalance(transaction.Amount);
                await _unitOfWork.Accounts.UpdateAsync(toAccount);
            }
            else
            {
                var toDeposit = await _unitOfWork.Deposits.GetByIdAsync(transaction.ToId);
                if (toDeposit != null)
                {
                    toDeposit.DecreaseBalance(transaction.Amount);
                    await _unitOfWork.Deposits.UpdateAsync(toDeposit);
                }
                else
                {
                    throw new InvalidOperationException($"Target entity with id {transaction.ToId} not found (neither account nor deposit)");
                }
            }
        }
        private async Task RevertClose(SystemLog log)
        {
            if (!log.TargetId.HasValue || !log.TargetType.HasValue)
                throw new InvalidOperationException("Invalid target type or target id in log");

            switch (log.TargetType.Value)
            {
                case LogTargetType.Account:
                    var account = await _unitOfWork.Accounts.GetByIdAsync(log.TargetId.Value);
                    if (account == null)
                        throw new InvalidOperationException($"Account with id {log.TargetId.Value} not found");

                    account.Reopen();
                    await _unitOfWork.Accounts.UpdateAsync(account);
                    break;

                case LogTargetType.Deposit:
                    var deposit = await _unitOfWork.Deposits.GetByIdAsync(log.TargetId.Value);
                    if (deposit == null)
                        throw new InvalidOperationException($"Deposit with id {log.TargetId.Value} not found");

                    deposit.Reopen();

                    if (!string.IsNullOrEmpty(log.ToInfo) && log.Amount.HasValue)
                    {
                        var accountNumber = log.ToInfo.Replace("Account ", "");
                        var acc = await _unitOfWork.Accounts.GetByNumberAsync(accountNumber);

                        if (acc != null)
                        {
                            acc.DecreaseBalance(log.Amount.Value);
                            deposit.IncreaseBalance(log.Amount.Value);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    break;

                default:
                    throw new InvalidOperationException($"Cannot revert close for target type {log.TargetType.Value}");
            }
        }
        private async Task RevertBlock(SystemLog log)
        {
            if (!log.TargetId.HasValue || !log.TargetType.HasValue)
                throw new InvalidOperationException("Invalid target type or target id in log");

            switch (log.TargetType.Value)
            {
                case LogTargetType.Account:
                    var account = await _unitOfWork.Accounts.GetByIdAsync(log.TargetId.Value);
                    if (account == null)
                        throw new InvalidOperationException($"Account with id {log.TargetId.Value} not found");

                    account.Unblock();
                    await _unitOfWork.Accounts.UpdateAsync(account);
                    break;

                case LogTargetType.Deposit:
                    var deposit = await _unitOfWork.Deposits.GetByIdAsync(log.TargetId.Value);
                    if (deposit == null)
                        throw new InvalidOperationException($"Deposit with id {log.TargetId.Value} not found");

                    deposit.Unblock(); 
                    await _unitOfWork.Deposits.UpdateAsync(deposit);
                    break;

                default:
                    throw new InvalidOperationException($"Cannot revert block for target type {log.TargetType.Value}");
            }
        }
        private async Task RevertUnblock(SystemLog log)
        {
            if (!log.TargetId.HasValue || !log.TargetType.HasValue)
                throw new InvalidOperationException("Invalid target type or target id in log");

            switch (log.TargetType.Value)
            {
                case LogTargetType.Account:
                    var account = await _unitOfWork.Accounts.GetByIdAsync(log.TargetId.Value);
                    if (account == null)
                        throw new InvalidOperationException($"Account with id {log.TargetId.Value} not found");

                    account.Block(); 
                    await _unitOfWork.Accounts.UpdateAsync(account);
                    break;

                case LogTargetType.Deposit:
                    var deposit = await _unitOfWork.Deposits.GetByIdAsync(log.TargetId.Value);
                    if (deposit == null)
                        throw new InvalidOperationException($"Deposit with id {log.TargetId.Value} not found");

                    deposit.Block(); 
                    await _unitOfWork.Deposits.UpdateAsync(deposit);
                    break;

                default:
                    throw new InvalidOperationException($"Cannot revert unblock for target type {log.TargetType.Value}");
            }
        }
        private async Task RevertApprove(SystemLog log)
        {
            if (!log.TargetId.HasValue || !log.TargetType.HasValue)
                throw new InvalidOperationException("Invalid target type or target id in log");

            switch (log.TargetType.Value)
            {
                case LogTargetType.Client:
                    var user = await _unitOfWork.Users.GetByIdAsync(log.TargetId.Value);
                    if (user == null)
                        throw new InvalidOperationException($"User with id {log.TargetId.Value} not found");

                    if (user is not Client client)
                        throw new InvalidOperationException($"User with id {log.TargetId.Value} is not a client");

                    client.RevokeApproval();
                    await _unitOfWork.Users.UpdateAsync(client);
                    break;

                case LogTargetType.SalaryProjectRequest:
                    var request = await _unitOfWork.SalaryRequests.GetByIdAsync(log.TargetId.Value);
                    if (request == null)
                        throw new InvalidOperationException($"Salary request with id {log.TargetId.Value} not found");

                    request.Reject();
                    await _unitOfWork.SalaryRequests.UpdateAsync(request);
                    break;

                default:
                    throw new InvalidOperationException($"Cannot revert approve for target type {log.TargetType.Value}");
            }
        }

        private async Task RevertReject(SystemLog log)
        {
            if (!log.TargetId.HasValue || !log.TargetType.HasValue)
                throw new InvalidOperationException("Invalid target type or target id in log");

            if (log.TargetType != LogTargetType.SalaryProjectRequest)
                throw new InvalidOperationException($"Cannot revert reject for target type {log.TargetType.Value}");

            var request = await _unitOfWork.SalaryRequests.GetByIdAsync(log.TargetId.Value);
            if (request == null)
                throw new InvalidOperationException($"Salary request with id {log.TargetId.Value} not found");

            request.Approve(); 
            await _unitOfWork.SalaryRequests.UpdateAsync(request);
        }

        private async Task RevertReceiveSalary(SystemLog log)
        {
            if (!log.TargetId.HasValue)
                throw new InvalidOperationException("Invalid target id in log");

            var transaction = await _unitOfWork.Transactions.GetByIdAsync(log.TargetId.Value);
            if (transaction == null)
                throw new InvalidOperationException($"Transaction with id {log.TargetId.Value} not found");

            if (transaction.ToId == Guid.Empty)
                throw new InvalidOperationException($"Transaction {transaction.Id} has empty ToId");

            var account = await _unitOfWork.Accounts.GetByIdAsync(transaction.ToId);
            if (account == null)
                throw new InvalidOperationException($"Account with id {transaction.ToId} not found");

            account.DecreaseBalance(transaction.Amount);
            await _unitOfWork.Accounts.UpdateAsync(account);
        }
    }
}
