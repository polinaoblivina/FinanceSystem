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
    public class ClientRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientRegistrationService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Client> Register(string login, string password, string firstName, string lastName)
        {
            if (await _unitOfWork.Users.GetByLoginAsync(login) != null)
                throw new InvalidOperationException("Login already exists");

            var client = Client.CreateNew(login, password, firstName, lastName);
            await _unitOfWork.Users.AddAsync(client);
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(client.Id, ActionType.Create));

            await _unitOfWork.SaveChangesAsync();
            return client;
        }
    }
}
