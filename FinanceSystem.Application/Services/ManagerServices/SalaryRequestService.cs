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
    public class SalaryRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SalaryRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ApproveSalaryRequest(Guid managerId, Guid requestId)
        {
            var request = await _unitOfWork.SalaryRequests.GetByIdAsync(requestId);
            if (request == null)
                throw new InvalidOperationException("Request not found");

            request.Approve();
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Approve, requestId, LogTargetType.SalaryProjectRequest));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectSalaryRequest(Guid managerId, Guid requestId)
        {
            var request = await _unitOfWork.SalaryRequests.GetByIdAsync(requestId);
            if (request == null)
                throw new InvalidOperationException("Request not found");

            request.Reject();
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Reject, requestId, LogTargetType.SalaryProjectRequest));
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
