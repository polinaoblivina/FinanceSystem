using System.Security.Claims;

namespace FinanceSystem.API.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers["X-UserId"].FirstOrDefault();
            if (header != null && Guid.TryParse(header, out var userId))
                return userId;

            throw new UnauthorizedAccessException("User ID not found in request.");
        }

        public string GetCurrentUserRole()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
            return claim?.Value ?? "Client";
        }
    }
}