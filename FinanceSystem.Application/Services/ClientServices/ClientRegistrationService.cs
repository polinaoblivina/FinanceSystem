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
    public class ClientRegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public ClientRegistrationService(IUserRepository userRepository, ILogRepository logRepository) 
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
        }
        public Client Register(string login, string password, string firstName, string lastName)
        {
            if (_userRepository.GetByLogin(login) != null)
                throw new InvalidOperationException("Login already exists");

            var client = Client.CreateNew(login, password, firstName, lastName);
            _userRepository.Add(client);

            _logRepository.Add(SystemLog.Create(client.Id, ActionType.Create));

            return client;
        }
    }
}
