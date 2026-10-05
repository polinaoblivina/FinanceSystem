using FinanceSystem.API.Contracts;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                var user = await _authService.Login(request.Login, request.Password);
                var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

                return Ok(new LoginResponse(token, expiresAt, user.Id, user.Login!, JwtTokenService.GetRole(user)));
            }
            catch (UnauthorizedAccessException)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Unauthorized", detail: "Invalid login or password.");
            }
        }
    }
}
