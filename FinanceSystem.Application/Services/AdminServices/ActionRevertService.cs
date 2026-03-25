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
                throw new InvalidOperationException($"Log with id {log.Id} not found");

            if (log.Action == ActionType.Login || log.Action == ActionType.Logout)
                throw new InvalidOperationException("Cannot revert login/logout actions");



            await _unitOfWork.SaveChangesAsync();
        }
    }
}
