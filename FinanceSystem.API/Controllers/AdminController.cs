using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinanceSystem.API.Services;
using FinanceSystem.Application.Services.AdminServices;

namespace FinanceSystem.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ActionRevertService _actionRevertService;
        private readonly LogViewerService _logViewerService;

        public AdminController(ActionRevertService actionRevertService, LogViewerService logViewerService)
        {
            _actionRevertService = actionRevertService;
            _logViewerService = logViewerService;
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
        public async Task<IActionResult> RevertAction(Guid adminId, string logNumber)
        {
            try
            {
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
