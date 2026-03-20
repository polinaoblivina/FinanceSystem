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
    public class ClientManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public ClientManagementService(IUserRepository userRepository, ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public List<User> GetAllClients()
        {
            return _userRepository.GetAll();
        }

        public void ApproveClient(Guid managerId, Guid clientId)
        {
            var user = _userRepository.GetById(clientId);
            if (user == null)
                throw new InvalidOperationException("User not found");
            if (user is not Client client)
                throw new InvalidOperationException("User is not a client");

            client.Approve();
            _userRepository.Update(client);

            _logRepository.Add(SystemLog.Create(managerId, ActionType.Approve, clientId, LogTargetType.Client));
        }
    }
}
