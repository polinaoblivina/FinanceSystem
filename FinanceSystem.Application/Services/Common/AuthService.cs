using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Repositories;

namespace FinanceSystem.Application.Services.Common
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        public AuthService(IUserRepository userRepository, ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public User Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Invalid login", nameof(login));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Invalid password", nameof(password));

            var user = _userRepository.GetByLogin(login);

            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            if (user.Password != password)
                throw new UnauthorizedAccessException("Invalid password");

            _logRepository.Add(SystemLog.Create(user.Id, ActionType.Login));
            return user;
        }

        public void Logout(Guid userId)
        {
            _logRepository.Add(SystemLog.Create(userId, ActionType.Logout));
        }
    }
}