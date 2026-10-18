using FinanceSystem.Application.Exceptions;
using FinanceSystem.Application.Services.AdminServices;
using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.ManagerServices;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Tests.Services
{
    public class ActionRevertServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly ActionRevertService _service;
        private readonly Guid _adminId = Guid.NewGuid();

        public ActionRevertServiceTests()
        {
            _service = new ActionRevertService(_db.UnitOfWork);
        }

        private async Task<SystemLog> LastLog(ActionType action)
        {
            var logs = await _db.UnitOfWork.Logs.GetAllAsync();
            return logs.Last(l => l.Action == action);
        }

        [Fact]
        public async Task Revert_Transfer_RestoresBalances()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var from = await _db.AddAccount(client.Id, bank.Id, 1000);
            var to = await _db.AddAccount(client.Id, bank.Id);
            await new TransferService(_db.UnitOfWork).TransferAccountToAccount(client.Id, from.Number!, to.Number!, 300);
            var log = await LastLog(ActionType.Transfer);

            await _service.RevertByLogId(_adminId, log.Number!);

            using var context = _db.CreateContext();
            Assert.Equal(1000, (await context.Accounts.FindAsync(from.Id))!.Balance);
            Assert.Equal(0, (await context.Accounts.FindAsync(to.Id))!.Balance);
            Assert.Contains(await context.Logs.ToListAsync(), l => l.Action == ActionType.Revert && l.UserId == _adminId);
        }

        [Fact]
        public async Task Revert_AccountToDepositTransfer_RestoresBalances()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id, 1000);
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 100);
            await new TransferService(_db.UnitOfWork).TransferAccountToDeposit(client.Id, account.Number!, deposit.Number!, 400);
            var log = await LastLog(ActionType.Transfer);

            await _service.RevertByLogId(_adminId, log.Number!);

            Assert.Equal(1000, account.Balance);
            Assert.Equal(100, deposit.Balance);
        }

        [Fact]
        public async Task Revert_AccountCreation_ClosesAccount()
        {
            var client = await _db.AddClient();
            await _db.AddBank("Bank1");
            var account = await new AccountService(_db.UnitOfWork).OpenAccount(client.Id, "Bank1");
            var log = await LastLog(ActionType.Create);

            await _service.RevertByLogId(_adminId, log.Number!);

            using var context = _db.CreateContext();
            Assert.Equal(AccountStatus.Closed, (await context.Accounts.FindAsync(account.Id))!.Status);
        }

        [Fact]
        public async Task Revert_AccountClose_ReopensAccount()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);
            await new AccountService(_db.UnitOfWork).CloseAccount(client.Id, account.Number!);
            var log = await LastLog(ActionType.Close);

            await _service.RevertByLogId(_adminId, log.Number!);

            Assert.Equal(AccountStatus.Active, account.Status);
        }

        [Fact]
        public async Task Revert_DepositClose_ReopensDepositAndTakesMoneyBack()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 500);
            await new DepositService(_db.UnitOfWork).CloseDeposit(client.Id, deposit.Number!, account.Number!);
            var log = await LastLog(ActionType.Close);

            await _service.RevertByLogId(_adminId, log.Number!);

            using var context = _db.CreateContext();
            var savedDeposit = (await context.Deposits.FindAsync(deposit.Id))!;
            Assert.Equal(DepositStatus.Active, savedDeposit.Status);
            Assert.Equal(500, savedDeposit.Balance);
            Assert.Equal(0, (await context.Accounts.FindAsync(account.Id))!.Balance);
        }

        [Fact]
        public async Task Revert_AccountBlock_UnblocksAccount()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);
            await new AccountControlService(_db.UnitOfWork).BlockAccount(Guid.NewGuid(), account.Number!);
            var log = await LastLog(ActionType.Block);

            await _service.RevertByLogId(_adminId, log.Number!);

            Assert.Equal(AccountStatus.Active, account.Status);
        }

        [Fact]
        public async Task Revert_DepositUnblock_BlocksDepositAgain()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 100);
            var control = new AccountControlService(_db.UnitOfWork);
            await control.BlockDeposit(Guid.NewGuid(), deposit.Number!);
            await control.UnblockDeposit(Guid.NewGuid(), deposit.Number!);
            var log = await LastLog(ActionType.Unblock);

            await _service.RevertByLogId(_adminId, log.Number!);

            Assert.Equal(DepositStatus.Blocked, deposit.Status);
        }

        [Fact]
        public async Task Revert_ClientApproval_RevokesApproval()
        {
            var client = await _db.AddClient("ivan", approved: false);
            await new ClientManagementService(_db.UnitOfWork).ApproveClient(Guid.NewGuid(), "ivan");
            var log = await LastLog(ActionType.Approve);

            await _service.RevertByLogId(_adminId, log.Number!);

            using var context = _db.CreateContext();
            Assert.False((await context.Users.OfType<Client>().SingleAsync()).IsApprovedByManager);
        }

        [Fact]
        public async Task Revert_ReceiveSalary_TakesSalaryBack()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var enterprise = await _db.AddEnterprise("Enterprise1");
            var account = await _db.AddAccount(client.Id, bank.Id);
            await _db.UnitOfWork.Employments.AddAsync(Employment.CreateNew(client.Id, enterprise.Id));
            var request = SalaryProjectRequest.CreateNew(client.Id, enterprise.Id);
            request.Approve();
            await _db.UnitOfWork.SalaryRequests.AddAsync(request);
            await _db.UnitOfWork.SaveChangesAsync();
            await new SalaryService(_db.UnitOfWork).ReceiveSalary(client.Id, "Enterprise1", account.Number!, 1000);
            var log = await LastLog(ActionType.ReceiveSalary);

            await _service.RevertByLogId(_adminId, log.Number!);

            Assert.Equal(0, account.Balance);
        }

        [Fact]
        public async Task Revert_InterestAccrual_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 100);
            await new DepositService(_db.UnitOfWork).AccrueInterest(client.Id, deposit.Number!);
            var log = await LastLog(ActionType.AccrueInterest);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RevertByLogId(_adminId, log.Number!));
        }

        [Fact]
        public async Task Revert_RevertLog_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);
            await new AccountControlService(_db.UnitOfWork).BlockAccount(Guid.NewGuid(), account.Number!);
            await _service.RevertByLogId(_adminId, (await LastLog(ActionType.Block)).Number!);
            var revertLog = await LastLog(ActionType.Revert);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RevertByLogId(_adminId, revertLog.Number!));
        }

        [Fact]
        public async Task Revert_UnknownLog_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.RevertByLogId(_adminId, "00000000"));
        }

        public void Dispose() => _db.Dispose();
    }

    public class LogViewerServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();

        [Fact]
        public async Task GetAllLogs_ReturnsAllLogs()
        {
            await _db.UnitOfWork.Logs.AddAsync(SystemLog.Create(Guid.NewGuid(), ActionType.Create));
            await _db.UnitOfWork.Logs.AddAsync(SystemLog.Create(Guid.NewGuid(), ActionType.Block));
            await _db.UnitOfWork.SaveChangesAsync();

            var logs = await new LogViewerService(_db.UnitOfWork).GetAllLogs();

            Assert.Equal(2, logs.Count);
        }

        public void Dispose() => _db.Dispose();
    }
}
