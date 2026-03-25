using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.ManagerServices;
using FInanceSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace FinanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly AccountControlService _accountControlService;
        private readonly ClientManagementService _clientManagementService;
        private readonly EnterpriseManagementService _enterpriseManagementService;
        private readonly SalaryRequestService _salaryRequestService;

        public ManagerController(AccountControlService accountControlService, ClientManagementService clientManagementService, EnterpriseManagementService enterpriseManagementService, SalaryRequestService salaryRequestService)
        {
            _accountControlService = accountControlService;
            _clientManagementService = clientManagementService;
            _enterpriseManagementService = enterpriseManagementService;
            _salaryRequestService = salaryRequestService;
        }

        [HttpPost("approve-client")]
        public async Task<IActionResult> ApproveClient(Guid managerId, string clientLogin)
        {
            try
            {
                await _clientManagementService.ApproveClient(managerId, clientLogin);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var clients = await _clientManagementService.GetAllClients();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-client-to-enterprise")]
        public async Task<IActionResult> AddClientToEnterprise(Guid managerId, string clientLogin, string enterpriseName)
        {
            try
            {
                await _enterpriseManagementService.AddClientToEnterprise(managerId, clientLogin, enterpriseName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("remove-client-from-enterprise")]
        public async Task<IActionResult> RemoveClientFromEnterprise(Guid managerId, string clientLogin, string enterpriseName)
        {
            try
            {
                await _enterpriseManagementService.RemoveClientFromEnterprise(managerId, clientLogin, enterpriseName);
                return Ok();
            }
            catch(Exception ex)
            {  
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("enterprises-with-employees")]
        public async Task<IActionResult> GetAllEnterprisesWithEmployees()
        {
            try
            {
                var enterprisesWithEmployees = await _enterpriseManagementService.GetAllEnterprisesWithEmployees();
                return Ok(enterprisesWithEmployees);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpPost("approve-salary-request")]
        public async Task<IActionResult> ApproveSalaryRequest(Guid managerId, string requestNumber)
        {
            try
            {
                await _salaryRequestService.ApproveSalaryRequest(managerId, requestNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reject-salary-request")]
        public async Task<IActionResult> RejectSalaryRequest(Guid managerId, string requestNumber)
        {
            try
            {
                await _salaryRequestService.RejectSalaryRequest(managerId, requestNumber);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("block-account")]
        public async Task<IActionResult> BlockAccount(Guid managerId, string accountNumber)
        {
            try
            {
                await _accountControlService.BlockAccount(managerId, accountNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("unblock-account")]
        public async Task<IActionResult> UnblockAccount(Guid managerId, string accountNumber)
        {
            try
            {
                await _accountControlService.UnblockAccount(managerId, accountNumber);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("block-deposit")]
        public async Task<IActionResult> BlockDeposit(Guid managerId, string depositNumber)
        {
            try
            {
                await _accountControlService.BlockDeposit(managerId, depositNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("unblock-deposit")]
        public async Task<IActionResult> UnblockDeposit(Guid managerId, string depositNumber)
        {
            try
            {
                await _accountControlService.UnblockDeposit(managerId, depositNumber);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("account-transactions")]
        public async Task<IActionResult> GetAccountTransactions(string accountNumber)
        {
            try
            {
                var transactions = await _accountControlService.GetAccountTransactions(accountNumber);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
