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
        public async Task<IActionResult> ApproveClient(Guid managerId, Guid clientId)
        {
            try
            {
                await _clientManagementService.ApproveClient(managerId, clientId);
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
        public async Task<IActionResult> AddClientToEnterprise(Guid managerId, Guid clientId, Guid enterpriseId)
        {
            try
            {
                await _enterpriseManagementService.AddClientToEnterprise(managerId, clientId, enterpriseId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("remove-client-from-enterprise")]
        public async Task<IActionResult> RemoveClientFromEnterprise(Guid managerId, Guid clientId, Guid enterpriseId)
        {
            try
            {
                await _enterpriseManagementService.RemoveClientFromEnterprise(managerId, clientId, enterpriseId);
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
        public async Task<IActionResult> ApproveSalaryRequest(Guid managerId, Guid requestId)
        {
            try
            {
                await _salaryRequestService.ApproveSalaryRequest(managerId, requestId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reject-salary-request")]
        public async Task<IActionResult> RejectSalaryRequest(Guid managerId, Guid requestId)
        {
            try
            {
                await _salaryRequestService.RejectSalaryRequest(managerId, requestId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("block-account")]
        public async Task<IActionResult> BlockAccount(Guid managerId, Guid accountId)
        {
            try
            {
                await _accountControlService.BlockAccount(managerId, accountId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("unblock-account")]
        public async Task<IActionResult> UnblockAccount(Guid managerId, Guid accountId)
        {
            try
            {
                await _accountControlService.UnblockAccount(managerId, accountId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("block-deposit")]
        public async Task<IActionResult> BlockDeposit(Guid managerId, Guid depositId)
        {
            try
            {
                await _accountControlService.BlockDeposit(managerId, depositId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("unblock-deposit")]
        public async Task<IActionResult> UnblockDeposit(Guid managerId, Guid depositId)
        {
            try
            {
                await _accountControlService.UnblockDeposit(managerId, depositId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("account-transactions")]
        public async Task<IActionResult> GetAccountTransactions(Guid accountId)
        {
            try
            {
                var transactions = await _accountControlService.GetAccountTransactions(accountId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
