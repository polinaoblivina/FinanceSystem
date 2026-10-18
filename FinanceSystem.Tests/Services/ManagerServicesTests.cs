using FinanceSystem.Application.Exceptions;
using FinanceSystem.Application.Services.ManagerServices;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Tests.Services
{
    public class AccountControlServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly AccountControlService _service;
        private readonly Guid _managerId = Guid.NewGuid();

        public AccountControlServiceTests()
        {
            _service = new AccountControlService(_db.UnitOfWork);
        }

        [Fact]
        public async Task BlockAndUnblockAccount_ChangeStatusAndWriteLogs()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);

            await _service.BlockAccount(_managerId, account.Number!);
            using (var context = _db.CreateContext())
                Assert.Equal(AccountStatus.Blocked, (await context.Accounts.SingleAsync()).Status);

            await _service.UnblockAccount(_managerId, account.Number!);
            using (var context = _db.CreateContext())
            {
                Assert.Equal(AccountStatus.Active, (await context.Accounts.SingleAsync()).Status);
                var actions = await context.Logs.Where(l => l.UserId == _managerId).Select(l => l.Action).ToListAsync();
                Assert.Contains(ActionType.Block, actions);
                Assert.Contains(ActionType.Unblock, actions);
            }
        }

        [Fact]
        public async Task BlockAccount_UnknownNumber_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.BlockAccount(_managerId, "000000"));
        }

        [Fact]
        public async Task BlockAndUnblockDeposit_ChangeStatus()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 100);

            await _service.BlockDeposit(_managerId, deposit.Number!);
            Assert.Equal(DepositStatus.Blocked, deposit.Status);

            await _service.UnblockDeposit(_managerId, deposit.Number!);
            Assert.Equal(DepositStatus.Active, deposit.Status);
        }

        [Fact]
        public async Task GetClientAccounts_ByLogin_ReturnsAccounts()
        {
            var client = await _db.AddClient("ivan");
            var bank = await _db.AddBank();
            await _db.AddAccount(client.Id, bank.Id);

            var accounts = await _service.GetClientAccounts("ivan");

            Assert.Single(accounts);
        }

        [Fact]
        public async Task GetClientAccounts_NonClientUser_Throws()
        {
            await _db.UnitOfWork.Users.AddAsync(Manager.CreateNew("boss", "hash", "Boss", "Bossov"));
            await _db.UnitOfWork.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetClientAccounts("boss"));
        }

        [Fact]
        public async Task GetClientDeposits_UnknownLogin_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetClientDeposits("nobody"));
        }

        public void Dispose() => _db.Dispose();
    }

    public class ClientManagementServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly ClientManagementService _service;

        public ClientManagementServiceTests()
        {
            _service = new ClientManagementService(_db.UnitOfWork);
        }

        [Fact]
        public async Task ApproveClient_SetsApprovedAndWritesLog()
        {
            var client = await _db.AddClient("ivan", approved: false);
            var managerId = Guid.NewGuid();

            await _service.ApproveClient(managerId, "ivan");

            using var context = _db.CreateContext();
            Assert.True((await context.Users.OfType<Client>().SingleAsync()).IsApprovedByManager);
            var log = await context.Logs.SingleAsync();
            Assert.Equal(ActionType.Approve, log.Action);
            Assert.Equal(client.Id, log.TargetId);
            Assert.Equal(LogTargetType.Client, log.TargetType);
        }

        [Fact]
        public async Task ApproveClient_AlreadyApproved_Throws()
        {
            await _db.AddClient("ivan", approved: true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ApproveClient(Guid.NewGuid(), "ivan"));
        }

        [Fact]
        public async Task ApproveClient_UnknownLogin_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ApproveClient(Guid.NewGuid(), "nobody"));
        }

        [Fact]
        public async Task GetAllClients_ReturnsOnlyClients()
        {
            await _db.AddClient("a");
            await _db.AddClient("b");
            await _db.UnitOfWork.Users.AddAsync(Admin.CreateNew("admin", "hash", "Admin", "Adminov"));
            await _db.UnitOfWork.SaveChangesAsync();

            var clients = await _service.GetAllClients();

            Assert.Equal(2, clients.Count);
        }

        public void Dispose() => _db.Dispose();
    }

    public class EnterpriseManagementServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly EnterpriseManagementService _service;
        private readonly Guid _managerId = Guid.NewGuid();

        public EnterpriseManagementServiceTests()
        {
            _service = new EnterpriseManagementService(_db.UnitOfWork);
        }

        [Fact]
        public async Task AddClientToEnterprise_CreatesEmployment()
        {
            var client = await _db.AddClient("ivan");
            var enterprise = await _db.AddEnterprise("Enterprise1");

            await _service.AddClientToEnterprise(_managerId, "ivan", "Enterprise1");

            using var context = _db.CreateContext();
            var employment = await context.Employments.SingleAsync();
            Assert.Equal(client.Id, employment.ClientId);
            Assert.Equal(enterprise.Id, employment.EnterpriseId);
        }

        [Fact]
        public async Task AddClientToEnterprise_Twice_Throws()
        {
            await _db.AddClient("ivan");
            await _db.AddEnterprise("Enterprise1");
            await _service.AddClientToEnterprise(_managerId, "ivan", "Enterprise1");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddClientToEnterprise(_managerId, "ivan", "Enterprise1"));
        }

        [Fact]
        public async Task AddClientToEnterprise_UnknownEnterprise_ThrowsNotFound()
        {
            await _db.AddClient("ivan");

            await Assert.ThrowsAsync<NotFoundException>(() => _service.AddClientToEnterprise(_managerId, "ivan", "Nope"));
        }

        [Fact]
        public async Task RemoveClientFromEnterprise_DeletesEmployment()
        {
            await _db.AddClient("ivan");
            await _db.AddEnterprise("Enterprise1");
            await _service.AddClientToEnterprise(_managerId, "ivan", "Enterprise1");

            await _service.RemoveClientFromEnterprise(_managerId, "ivan", "Enterprise1");

            using var context = _db.CreateContext();
            Assert.Empty(await context.Employments.ToListAsync());
            Assert.Contains(await context.Logs.ToListAsync(), l => l.Action == ActionType.Delete);
        }

        [Fact]
        public async Task RemoveClientFromEnterprise_NotEmployed_Throws()
        {
            await _db.AddClient("ivan");
            await _db.AddEnterprise("Enterprise1");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoveClientFromEnterprise(_managerId, "ivan", "Enterprise1"));
        }

        [Fact]
        public async Task GetAllEnterprisesWithEmployees_GroupsClients()
        {
            await _db.AddClient("ivan");
            await _db.AddClient("petr");
            await _db.AddEnterprise("Enterprise1");
            await _db.AddEnterprise("Enterprise2");
            await _service.AddClientToEnterprise(_managerId, "ivan", "Enterprise1");
            await _service.AddClientToEnterprise(_managerId, "petr", "Enterprise1");

            var result = await _service.GetAllEnterprisesWithEmployees();

            Assert.Equal(2, result.Count);
            Assert.Equal(2, result.Single(r => r.Item1.Name == "Enterprise1").Item2.Count);
            Assert.Empty(result.Single(r => r.Item1.Name == "Enterprise2").Item2);
        }

        public void Dispose() => _db.Dispose();
    }

    public class SalaryRequestServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly SalaryRequestService _service;

        public SalaryRequestServiceTests()
        {
            _service = new SalaryRequestService(_db.UnitOfWork);
        }

        private async Task<SalaryProjectRequest> AddPendingRequest()
        {
            var request = SalaryProjectRequest.CreateNew(Guid.NewGuid(), Guid.NewGuid());
            await _db.UnitOfWork.SalaryRequests.AddAsync(request);
            await _db.UnitOfWork.SaveChangesAsync();
            return request;
        }

        [Fact]
        public async Task ApproveSalaryRequest_SetsAccepted()
        {
            var request = await AddPendingRequest();

            await _service.ApproveSalaryRequest(Guid.NewGuid(), request.Number!);

            using var context = _db.CreateContext();
            Assert.Equal(RequestStatus.Accepted, (await context.SalaryProjectRequests.SingleAsync()).Status);
        }

        [Fact]
        public async Task RejectSalaryRequest_SetsRejected()
        {
            var request = await AddPendingRequest();

            await _service.RejectSalaryRequest(Guid.NewGuid(), request.Number!);

            using var context = _db.CreateContext();
            Assert.Equal(RequestStatus.Rejected, (await context.SalaryProjectRequests.SingleAsync()).Status);
        }

        [Fact]
        public async Task GetPendingRequests_ExcludesProcessed()
        {
            await AddPendingRequest();
            var processed = await AddPendingRequest();
            await _service.ApproveSalaryRequest(Guid.NewGuid(), processed.Number!);

            var pending = await _service.GetPendingRequests();

            Assert.Single(pending);
            Assert.NotEqual(processed.Id, pending[0].Id);
        }

        [Fact]
        public async Task ApproveSalaryRequest_UnknownNumber_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ApproveSalaryRequest(Guid.NewGuid(), "00000000"));
        }

        public void Dispose() => _db.Dispose();
    }
}
