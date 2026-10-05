using FinanceSystem.API.Services;
using FinanceSystem.Application.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetLogs()
        {
            try
            {
                var logs = await _logViewerService.GetAllLogs();
                return Ok(logs);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("revert-action")]
        public async Task<IActionResult> RevertAction(string logNumber)
        {
            try
            {
                var adminId = _currentUserService.GetCurrentUserId();
                await _actionRevertService.RevertByLogId(adminId, logNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
