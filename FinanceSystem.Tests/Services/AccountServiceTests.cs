using FinanceSystem.Application.Exceptions;
using FinanceSystem.Application.Services.ClientServices;
using FInanceSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Tests.Services
{
    public class AccountServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            _service = new AccountService(_db.UnitOfWork);
        }

        [Fact]
        public async Task OpenAccount_ApprovedClient_CreatesAccountAndLog()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank("Bank1");

            var account = await _service.OpenAccount(client.Id, "Bank1");

            using var context = _db.CreateContext();
            var saved = await context.Accounts.SingleAsync();
            Assert.Equal(account.Id, saved.Id);
            Assert.Equal(client.Id, saved.ClientId);
            Assert.Equal(bank.Id, saved.BankId);
            Assert.Equal(AccountStatus.Active, saved.Status);

            var log = await context.Logs.SingleAsync();
            Assert.Equal(ActionType.Create, log.Action);
            Assert.Equal(account.Id, log.TargetId);
            Assert.Equal(LogTargetType.Account, log.TargetType);
        }

        [Fact]
        public async Task OpenAccount_NotApprovedClient_Throws()
        {
            var client = await _db.AddClient(approved: false);
            await _db.AddBank("Bank1");

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.OpenAccount(client.Id, "Bank1"));
        }

        [Fact]
        public async Task OpenAccount_UnknownBank_ThrowsNotFound()
        {
            var client = await _db.AddClient();

            await Assert.ThrowsAsync<NotFoundException>(() => _service.OpenAccount(client.Id, "NoSuchBank"));
        }

        [Fact]
        public async Task CloseAccount_EmptyOwnAccount_ClosesIt()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);

            await _service.CloseAccount(client.Id, account.Number!);

            using var context = _db.CreateContext();
            var saved = await context.Accounts.SingleAsync();
            Assert.Equal(AccountStatus.Closed, saved.Status);
            Assert.Contains(await context.Logs.ToListAsync(), l => l.Action == ActionType.Close && l.TargetId == account.Id);
        }

        [Fact]
        public async Task CloseAccount_ForeignAccount_Throws()
        {
            var owner = await _db.AddClient("owner");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(owner.Id, bank.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CloseAccount(other.Id, account.Number!));
            Assert.Equal(AccountStatus.Active, account.Status);
        }

        [Fact]
        public async Task CloseAccount_WithBalance_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id, 100);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CloseAccount(client.Id, account.Number!));
        }

        [Fact]
        public async Task CloseAccount_UnknownNumber_ThrowsNotFound()
        {
            var client = await _db.AddClient();

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CloseAccount(client.Id, "000000"));
        }

        [Fact]
        public async Task GetClientAccounts_ReturnsOnlyOwnAccounts()
        {
            var client = await _db.AddClient("owner");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            await _db.AddAccount(client.Id, bank.Id);
            await _db.AddAccount(client.Id, bank.Id);
            await _db.AddAccount(other.Id, bank.Id);

            var accounts = await _service.GetClientAccounts(client.Id);

            Assert.Equal(2, accounts.Count);
            Assert.All(accounts, a => Assert.Equal(client.Id, a.ClientId));
        }

        [Fact]
        public async Task GetTransactions_ForeignAccount_Throws()
        {
            var owner = await _db.AddClient("owner");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(owner.Id, bank.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetTransactions(other.Id, account.Number!));
        }

        public void Dispose() => _db.Dispose();
    }
}
