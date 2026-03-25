using FinanceSystem.Application.Services.ClientServices;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ClientRegistrationService _clientRegistrationService;
        private readonly AccountService _accountService;
        private readonly DepositService _depositService;
        private readonly TransferService _transferService;
        private readonly InfoService _infoService;
        private readonly SalaryService _salaryService;

        public ClientController(ClientRegistrationService clientRegistrationService, AccountService accountService, DepositService depositService, TransferService transferService, InfoService infoService, SalaryService salaryService)
        {
            _clientRegistrationService = clientRegistrationService;
            _accountService = accountService;
            _depositService = depositService;
            _transferService = transferService;
            _infoService = infoService;
            _salaryService = salaryService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string login, string password, string firstName, string lastName)
        {
            try
            {
                var client = await _clientRegistrationService.Register(login, password, firstName, lastName);
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("banks")]
        public async Task<IActionResult> GetBanks()
        {
            try
            {
                var banks = await _infoService.GetBanks();
                return Ok(banks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("enterprises")]
        public async Task<IActionResult> GetEnterprises()
        {
            try
            {
                var enterprises = await _infoService.GetEnterprises();
                return Ok(enterprises);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("open-account")]
        public async Task<IActionResult> OpenAccount(Guid clientId, string bankName)
        {
            try
            {
                var account = await _accountService.OpenAccount(clientId, bankName);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("close-account")]
        public async Task<IActionResult> CloseAccount(Guid clientId, string accountNumber)
        {
            try
            {
                await _accountService.CloseAccount(clientId, accountNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("account-transactions")]
        public async Task<IActionResult> GetAccountTransactions(Guid clientId, string accountNumber)
        {
            try
            {
                var transactions = await _accountService.GetTransactions(clientId, accountNumber);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-deposit")]
        public async Task<IActionResult> CreateDeposit(Guid clientId, string accountNumber, decimal amount, double rate, DepositType type)
        {
            try
            {
                var deposit = await _depositService.CreateDeposit(clientId, accountNumber, amount, rate, type);
                return Ok(deposit);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("close-deposit")]
        public async Task<IActionResult> CloseDeposit(Guid clientId, string depositNumber, string accountNumber)
        {
            try
            {
                await _depositService.CloseDeposit(clientId, depositNumber, accountNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("accrue-interest")]
        public async Task<IActionResult> AccrueInterest(Guid clientId, string depositNumber)
        {
            try
            {
                await _depositService.AccrueInterest(clientId, depositNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer-account-to-account")]
        public async Task<IActionResult> TransferAccountToAccount(Guid clientId, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            try
            {
                await _transferService.TransferAccountToAccount(clientId, fromAccountNumber, toAccountNumber, amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer-account-to-deposit")]
        public async Task<IActionResult> TransferAccountToDeposit(Guid clientId, string accountNumber, string depositNumber, decimal amount)
        {
            try
            {
                await _transferService.TransferAccountToDeposit(clientId, accountNumber, depositNumber, amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer-deposit-to-account")]
        public async Task<IActionResult> TransferDepositToAccount(Guid clientId, string depositNumber, string accountNumber, decimal amount)
        {
            try
            {
                await _transferService.TransferDepositToAccount(clientId, depositNumber, accountNumber, amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer-deposit-to-deposit")]
        public async Task<IActionResult> TransferDepositToDeposit(Guid clientId, string fromDepositNumber, string toDepositNumber, decimal amount)
        {
            try
            {
                await _transferService.TransferDepositToDeposit(clientId, fromDepositNumber, toDepositNumber, amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("request-salary-project")]
        public async Task<IActionResult> RequestSalaryProject(Guid clientId, string enterpriseName)
        {
            try
            {
                await _salaryService.RequestSalaryProject(clientId, enterpriseName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("receive-salary")]
        public async Task<IActionResult> ReceiveSalary(Guid clientId, string enterpriseName, string accountNumber, decimal amount)
        {
            try
            {
                await _salaryService.ReceiveSalary(clientId, enterpriseName, accountNumber, amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
