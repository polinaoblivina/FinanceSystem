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
        public List<SystemLog> GetAllLogs() => _logViewerService.GetAllLogs();
        public void RevertByLogId(Guid adminId, Guid logId) => _actionRevertService.RevertByLogId(adminId, logId);
    }
}
