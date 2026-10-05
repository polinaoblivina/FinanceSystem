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
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (value != null && Guid.TryParse(value, out var userId))
                return userId;

            throw new UnauthorizedAccessException("User is not authenticated.");
        }
    }
}
