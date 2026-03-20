using FinanceSystem.Application.Services.AdminServices;
using FInanceSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSystem.Application.Facades
{
    public class AdminFacade
    {

        private readonly LogViewerService _logViewerService;
        private readonly ActionRevertService _actionRevertService;

        public AdminFacade(LogViewerService logViewerService,ActionRevertService actionRevertService)
        {
            _logViewerService = logViewerService;
            _actionRevertService = actionRevertService;
        }
        public async Task<List<SystemLog>> GetAllLogs() => await _logViewerService.GetAllLogs();
        public async Task RevertByLogId(Guid adminId, Guid logId) => await  _actionRevertService.RevertByLogId(adminId, logId);
    }
}
