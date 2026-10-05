using FinanceSystem.API.Contracts;
using FinanceSystem.API.Services;
using FinanceSystem.Application.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly ActionRevertService _actionRevertService;
        private readonly LogViewerService _logViewerService;
        private readonly CurrentUserService _currentUserService;

        public AdminController(ActionRevertService actionRevertService, LogViewerService logViewerService, CurrentUserService currentUserService)
        {
            _actionRevertService = actionRevertService;
            _logViewerService = logViewerService;
            _currentUserService = currentUserService;
        }

        [HttpGet("logs")]
        public async Task<ActionResult<List<LogResponse>>> GetLogs()
        {
            var logs = await _logViewerService.GetAllLogs();
            return Ok(logs.Select(LogResponse.From).ToList());
        }

        [HttpPost("revert-action")]
        public async Task<IActionResult> RevertAction(RevertActionRequest request)
        {
            var adminId = _currentUserService.GetCurrentUserId();
            await _actionRevertService.RevertByLogId(adminId, request.LogNumber);
            return NoContent();
        }
    }
}
