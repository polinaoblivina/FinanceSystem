using FinanceSystem.API.Contracts;
using FinanceSystem.API.Services;
using FinanceSystem.Application.Services.ManagerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Manager)]
    public class ManagerController : ControllerBase
    {
        private readonly AccountControlService _accountControlService;
        private readonly ClientManagementService _clientManagementService;
        private readonly EnterpriseManagementService _enterpriseManagementService;
        private readonly SalaryRequestService _salaryRequestService;
        private readonly CurrentUserService _currentUserService;

        public ManagerController(AccountControlService accountControlService, ClientManagementService clientManagementService, EnterpriseManagementService enterpriseManagementService, SalaryRequestService salaryRequestService, CurrentUserService currentUserService)
        {
            _accountControlService = accountControlService;
            _clientManagementService = clientManagementService;
            _enterpriseManagementService = enterpriseManagementService;
            _salaryRequestService = salaryRequestService;
            _currentUserService = currentUserService;
        }

        [HttpPost("approve-client")]
        public async Task<IActionResult> ApproveClient(ClientLoginRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _clientManagementService.ApproveClient(managerId, request.ClientLogin);
            return NoContent();
        }

        [HttpGet("clients")]
        public async Task<ActionResult<List<ClientResponse>>> GetAllClients()
        {
            var clients = await _clientManagementService.GetAllClients();
            return Ok(clients.Select(ClientResponse.From).ToList());
        }

        [HttpPost("add-client-to-enterprise")]
        public async Task<IActionResult> AddClientToEnterprise(ClientEnterpriseRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _enterpriseManagementService.AddClientToEnterprise(managerId, request.ClientLogin, request.EnterpriseName);
            return NoContent();
        }

        [HttpPost("remove-client-from-enterprise")]
        public async Task<IActionResult> RemoveClientFromEnterprise(ClientEnterpriseRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _enterpriseManagementService.RemoveClientFromEnterprise(managerId, request.ClientLogin, request.EnterpriseName);
            return NoContent();
        }

        [HttpGet("enterprises-with-employees")]
        public async Task<ActionResult<List<EnterpriseEmployeesResponse>>> GetAllEnterprisesWithEmployees()
        {
            var enterprises = await _enterpriseManagementService.GetAllEnterprisesWithEmployees();
            return Ok(enterprises
                .Select(item => EnterpriseEmployeesResponse.From(item.Item1, item.Item2))
                .ToList());
        }

        [HttpGet("pending-salary-requests")]
        public async Task<ActionResult<List<SalaryRequestResponse>>> GetPendingSalaryRequests()
        {
            var requests = await _salaryRequestService.GetPendingRequests();
            return Ok(requests.Select(SalaryRequestResponse.From).ToList());
        }

        [HttpPost("approve-salary-request")]
        public async Task<IActionResult> ApproveSalaryRequest(SalaryRequestNumberRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _salaryRequestService.ApproveSalaryRequest(managerId, request.RequestNumber);
            return NoContent();
        }

        [HttpPost("reject-salary-request")]
        public async Task<IActionResult> RejectSalaryRequest(SalaryRequestNumberRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _salaryRequestService.RejectSalaryRequest(managerId, request.RequestNumber);
            return NoContent();
        }

        [HttpPost("block-account")]
        public async Task<IActionResult> BlockAccount(AccountNumberRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _accountControlService.BlockAccount(managerId, request.AccountNumber);
            return NoContent();
        }

        [HttpPost("unblock-account")]
        public async Task<IActionResult> UnblockAccount(AccountNumberRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _accountControlService.UnblockAccount(managerId, request.AccountNumber);
            return NoContent();
        }

        [HttpPost("block-deposit")]
        public async Task<IActionResult> BlockDeposit(DepositNumberRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _accountControlService.BlockDeposit(managerId, request.DepositNumber);
            return NoContent();
        }

        [HttpPost("unblock-deposit")]
        public async Task<IActionResult> UnblockDeposit(DepositNumberRequest request)
        {
            var managerId = _currentUserService.GetCurrentUserId();
            await _accountControlService.UnblockDeposit(managerId, request.DepositNumber);
            return NoContent();
        }

        [HttpGet("client-accounts")]
        public async Task<ActionResult<List<AccountResponse>>> GetClientAccounts([FromQuery] string clientLogin)
        {
            var accounts = await _accountControlService.GetClientAccounts(clientLogin);
            return Ok(accounts.Select(AccountResponse.From).ToList());
        }

        [HttpGet("client-deposits")]
        public async Task<ActionResult<List<DepositResponse>>> GetClientDeposits([FromQuery] string clientLogin)
        {
            var deposits = await _accountControlService.GetClientDeposits(clientLogin);
            return Ok(deposits.Select(DepositResponse.From).ToList());
        }

        [HttpGet("account-transactions")]
        public async Task<ActionResult<List<TransactionResponse>>> GetAccountTransactions([FromQuery] string accountNumber)
        {
            var transactions = await _accountControlService.GetAccountTransactions(accountNumber);
            return Ok(transactions.Select(TransactionResponse.From).ToList());
        }
    }
}
