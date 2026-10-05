using FInanceSystem.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinanceSystem.API.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetRole(User user)
        {
            return user switch
            {
                Admin => Roles.Admin,
                Manager => Roles.Manager,
                _ => Roles.Client
            };
        }

        public (string Token, DateTime ExpiresAt) GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue("Jwt:ExpiresMinutes", 60));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login ?? string.Empty),
                new Claim(ClaimTypes.Role, GetRole(user))
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
