using FinanceSystem.Application.Exceptions;
using FinanceSystem.Application.Services.ClientServices;
using FInanceSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Tests.Services
{
    public class DepositServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly DepositService _service;

        public DepositServiceTests()
        {
            _service = new DepositService(_db.UnitOfWork);
        }

        [Fact]
        public async Task CreateDeposit_MovesMoneyFromAccount()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id, 1000);

            var deposit = await _service.CreateDeposit(client.Id, account.Number!, 400, 0.1, DepositType.OpenDeposit);

            using var context = _db.CreateContext();
            Assert.Equal(600, (await context.Accounts.SingleAsync()).Balance);

            var savedDeposit = await context.Deposits.SingleAsync();
            Assert.Equal(deposit.Id, savedDeposit.Id);
            Assert.Equal(400, savedDeposit.Balance);
            Assert.Equal(bank.Id, savedDeposit.BankId);

            var transaction = await context.Transactions.SingleAsync();
            Assert.Equal(account.Id, transaction.FromId);
            Assert.Equal(deposit.Id, transaction.ToId);
            Assert.Equal(TransactionType.TransferAccountToDeposit, transaction.Type);
        }

        [Fact]
        public async Task CreateDeposit_InsufficientFunds_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id, 100);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDeposit(client.Id, account.Number!, 500, 0.1, DepositType.OpenDeposit));

            using var context = _db.CreateContext();
            Assert.Empty(await context.Deposits.ToListAsync());
        }

        [Fact]
        public async Task CreateDeposit_ForeignAccount_Throws()
        {
            var owner = await _db.AddClient("owner");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(owner.Id, bank.Id, 1000);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateDeposit(other.Id, account.Number!, 100, 0.1, DepositType.OpenDeposit));
        }

        [Fact]
        public async Task CreateDeposit_NotApprovedClient_Throws()
        {
            var client = await _db.AddClient(approved: false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateDeposit(client.Id, "123456", 100, 0.1, DepositType.OpenDeposit));
        }

        [Fact]
        public async Task CloseDeposit_ReturnsMoneyToAccount()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 500);

            await _service.CloseDeposit(client.Id, deposit.Number!, account.Number!);

            using var context = _db.CreateContext();
            var savedDeposit = await context.Deposits.SingleAsync();
            Assert.Equal(DepositStatus.Closed, savedDeposit.Status);
            Assert.Equal(0, savedDeposit.Balance);
            Assert.Equal(500, (await context.Accounts.SingleAsync()).Balance);

            var log = await context.Logs.SingleAsync();
            Assert.Equal(ActionType.Close, log.Action);
            Assert.Equal(500, log.Amount);
            Assert.Equal($"Account {account.Number}", log.ToInfo);
        }

        [Fact]
        public async Task CloseDeposit_ForeignDeposit_Throws()
        {
            var owner = await _db.AddClient("owner");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(other.Id, bank.Id);
            var deposit = await _db.AddDeposit(owner.Id, bank.Id, 500);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CloseDeposit(other.Id, deposit.Number!, account.Number!));
        }

        [Fact]
        public async Task CloseDeposit_UnknownDeposit_ThrowsNotFound()
        {
            var client = await _db.AddClient();

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CloseDeposit(client.Id, "000000", "111111"));
        }

        [Fact]
        public async Task AccrueInterest_WritesLog()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 500);

            await _service.AccrueInterest(client.Id, deposit.Number!);

            using var context = _db.CreateContext();
            var log = await context.Logs.SingleAsync();
            Assert.Equal(ActionType.AccrueInterest, log.Action);
            Assert.Equal(deposit.Id, log.TargetId);
        }

        [Fact]
        public async Task AccrueInterest_ForeignDeposit_Throws()
        {
            var owner = await _db.AddClient("owner");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            var deposit = await _db.AddDeposit(owner.Id, bank.Id, 500);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.AccrueInterest(other.Id, deposit.Number!));
        }

        [Fact]
        public async Task GetClientDeposits_ReturnsOnlyOwnDeposits()
        {
            var client = await _db.AddClient("owner");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            await _db.AddDeposit(client.Id, bank.Id, 100);
            await _db.AddDeposit(other.Id, bank.Id, 100);

            var deposits = await _service.GetClientDeposits(client.Id);

            Assert.Single(deposits);
            Assert.Equal(client.Id, deposits[0].ClientId);
        }

        public void Dispose() => _db.Dispose();
    }
}
