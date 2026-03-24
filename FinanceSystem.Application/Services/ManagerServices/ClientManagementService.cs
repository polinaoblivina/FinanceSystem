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
    public class ClientManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ClientManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Client>> GetAllClients()
        {
            return await _unitOfWork.Users.GetAllClients();
        }

        public async Task ApproveClient(Guid managerId, Guid clientId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(clientId);
            if (user == null)
                throw new InvalidOperationException("User not found");
            if (user is not Client client)
                throw new InvalidOperationException("User is not a client");

            client.Approve();

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(managerId, ActionType.Approve, clientId, LogTargetType.Client));
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
