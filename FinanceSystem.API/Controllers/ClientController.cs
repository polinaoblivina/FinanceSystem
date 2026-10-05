using FinanceSystem.API.Contracts;
using FinanceSystem.API.Services;
using FinanceSystem.Application.Services.ClientServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Client)]
    public class ClientController : ControllerBase
    {
        private readonly ClientRegistrationService _clientRegistrationService;
        private readonly AccountService _accountService;
        private readonly DepositService _depositService;
        private readonly TransferService _transferService;
        private readonly InfoService _infoService;
        private readonly SalaryService _salaryService;
        private readonly CurrentUserService _currentUserService;

        public ClientController(ClientRegistrationService clientRegistrationService, AccountService accountService, DepositService depositService, TransferService transferService, InfoService infoService, SalaryService salaryService, CurrentUserService currentUserService)
        {
            _clientRegistrationService = clientRegistrationService;
            _accountService = accountService;
            _depositService = depositService;
            _transferService = transferService;
            _infoService = infoService;
            _salaryService = salaryService;
            _currentUserService = currentUserService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ClientResponse>> Register(RegisterRequest request)
        {
            var client = await _clientRegistrationService.Register(request.Login, request.Password, request.FirstName, request.LastName);
            return StatusCode(StatusCodes.Status201Created, ClientResponse.From(client));
        }

        [AllowAnonymous]
        [HttpGet("banks")]
        public async Task<ActionResult<List<BankResponse>>> GetBanks()
        {
            var banks = await _infoService.GetBanks();
            return Ok(banks.Select(BankResponse.From).ToList());
        }

        [AllowAnonymous]
        [HttpGet("enterprises")]
        public async Task<ActionResult<List<EnterpriseResponse>>> GetEnterprises()
        {
            var enterprises = await _infoService.GetEnterprises();
            return Ok(enterprises.Select(EnterpriseResponse.From).ToList());
        }

        [HttpGet("accounts")]
        public async Task<ActionResult<List<AccountResponse>>> GetAccounts()
        {
            var clientId = _currentUserService.GetCurrentUserId();
            var accounts = await _accountService.GetClientAccounts(clientId);
            return Ok(accounts.Select(AccountResponse.From).ToList());
        }

        [HttpGet("deposits")]
        public async Task<ActionResult<List<DepositResponse>>> GetDeposits()
        {
            var clientId = _currentUserService.GetCurrentUserId();
            var deposits = await _depositService.GetClientDeposits(clientId);
            return Ok(deposits.Select(DepositResponse.From).ToList());
        }

        [HttpPost("open-account")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<AccountResponse>> OpenAccount(OpenAccountRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            var account = await _accountService.OpenAccount(clientId, request.BankName);
            return StatusCode(StatusCodes.Status201Created, AccountResponse.From(account));
        }

        [HttpPost("close-account")]
        public async Task<IActionResult> CloseAccount(AccountNumberRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _accountService.CloseAccount(clientId, request.AccountNumber);
            return NoContent();
        }

        [HttpGet("account-transactions")]
        public async Task<ActionResult<List<TransactionResponse>>> GetAccountTransactions([FromQuery] string accountNumber)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            var transactions = await _accountService.GetTransactions(clientId, accountNumber);
            return Ok(transactions.Select(TransactionResponse.From).ToList());
        }

        [HttpPost("create-deposit")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<DepositResponse>> CreateDeposit(CreateDepositRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            var deposit = await _depositService.CreateDeposit(clientId, request.AccountNumber, request.Amount, request.Rate, request.Type);
            return StatusCode(StatusCodes.Status201Created, DepositResponse.From(deposit));
        }

        [HttpPost("close-deposit")]
        public async Task<IActionResult> CloseDeposit(CloseDepositRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _depositService.CloseDeposit(clientId, request.DepositNumber, request.AccountNumber);
            return NoContent();
        }

        [HttpPost("accrue-interest")]
        public async Task<IActionResult> AccrueInterest(DepositNumberRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _depositService.AccrueInterest(clientId, request.DepositNumber);
            return NoContent();
        }

        [HttpPost("transfer-account-to-account")]
        public async Task<IActionResult> TransferAccountToAccount(TransferRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _transferService.TransferAccountToAccount(clientId, request.From, request.To, request.Amount);
            return NoContent();
        }

        [HttpPost("transfer-account-to-deposit")]
        public async Task<IActionResult> TransferAccountToDeposit(TransferRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _transferService.TransferAccountToDeposit(clientId, request.From, request.To, request.Amount);
            return NoContent();
        }

        [HttpPost("transfer-deposit-to-account")]
        public async Task<IActionResult> TransferDepositToAccount(TransferRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _transferService.TransferDepositToAccount(clientId, request.From, request.To, request.Amount);
            return NoContent();
        }

        [HttpPost("transfer-deposit-to-deposit")]
        public async Task<IActionResult> TransferDepositToDeposit(TransferRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _transferService.TransferDepositToDeposit(clientId, request.From, request.To, request.Amount);
            return NoContent();
        }

        [HttpPost("request-salary-project")]
        public async Task<IActionResult> RequestSalaryProject(EnterpriseRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _salaryService.RequestSalaryProject(clientId, request.EnterpriseName);
            return NoContent();
        }

        [HttpPost("receive-salary")]
        public async Task<IActionResult> ReceiveSalary(ReceiveSalaryRequest request)
        {
            var clientId = _currentUserService.GetCurrentUserId();
            await _salaryService.ReceiveSalary(clientId, request.EnterpriseName, request.AccountNumber, request.Amount);
            return NoContent();
        }
    }
}
