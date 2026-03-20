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
    public class SalaryRequestService
    {
        private readonly ISalaryProjectRequestRepository _requestRepository;
        private readonly ILogRepository _logRepository;

        public SalaryRequestService(ISalaryProjectRequestRepository requestRepository, ILogRepository logRepository) 
        {
            _requestRepository = requestRepository;
            _logRepository = logRepository;
        }

        public void ApproveSalaryRequest(Guid managerId, Guid requestId)
        {
            var request = _requestRepository.GetById(requestId);
            if (request == null)
                throw new InvalidOperationException("Request not found");

            request.Approve();
            _requestRepository.Update(request);

            _logRepository.Add(SystemLog.Create(managerId, ActionType.Approve, requestId, LogTargetType.SalaryProjectRequest));
        }

        public void RejectSalaryRequest(Guid managerId, Guid requestId)
        {
            var request = _requestRepository.GetById(requestId);
            if (request == null)
                throw new InvalidOperationException("Request not found");

            request.Reject();
            _requestRepository.Update(request);

            _logRepository.Add(SystemLog.Create(managerId, ActionType.Reject, requestId, LogTargetType.SalaryProjectRequest));
        }
    }
}
