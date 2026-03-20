using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Interfaces;
using FInanceSystem.Domain.Repositories;

namespace FinanceSystem.Application.Services.Common
{
    public class AuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Invalid login", nameof(login));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Invalid password", nameof(password));

            var user = await _unitOfWork.Users.GetByLoginAsync(login);

            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            if (user.Password != password)
                throw new UnauthorizedAccessException("Invalid password");

            await _unitOfWork.Logs.AddAsync(SystemLog.Create(user.Id, ActionType.Login));
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task Logout(Guid userId)
        {
            await _unitOfWork.Logs.AddAsync(SystemLog.Create(userId, ActionType.Logout));
            await _unitOfWork.SaveChangesAsync();
        }
    }
}