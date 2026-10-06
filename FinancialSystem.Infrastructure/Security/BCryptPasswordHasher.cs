using FinanceSystem.Application.Interfaces;

namespace FinanceSystem.Infrastructure.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 11;

        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        public bool Verify(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }
    }
}
