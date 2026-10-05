using FinanceSystem.API.Services;
using FinanceSystem.Application.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(AuthService authService, JwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string login, string password)
        {
            try
            {
                var user = await _authService.Login(login, password);
                var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

                return Ok(new
                {
                    token,
                    expiresAt,
                    userId = user.Id,
                    login = user.Login,
                    role = JwtTokenService.GetRole(user)
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
