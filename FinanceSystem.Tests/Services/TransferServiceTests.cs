using FinanceSystem.Application.Exceptions;
using FinanceSystem.Application.Services.ClientServices;
using FInanceSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Tests.Services
{
    public class TransferServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly TransferService _service;

        public TransferServiceTests()
        {
            _service = new TransferService(_db.UnitOfWork);
        }

        [Fact]
        public async Task TransferAccountToAccount_MovesMoneyAndWritesTransaction()
        {
            var sender = await _db.AddClient("sender");
            var receiver = await _db.AddClient("receiver");
            var bank = await _db.AddBank();
            var from = await _db.AddAccount(sender.Id, bank.Id, 1000);
            var to = await _db.AddAccount(receiver.Id, bank.Id, 50);

            await _service.TransferAccountToAccount(sender.Id, from.Number!, to.Number!, 300);

            using var context = _db.CreateContext();
            Assert.Equal(700, (await context.Accounts.FindAsync(from.Id))!.Balance);
            Assert.Equal(350, (await context.Accounts.FindAsync(to.Id))!.Balance);

            var transaction = await context.Transactions.SingleAsync();
            Assert.Equal(from.Id, transaction.FromId);
            Assert.Equal(to.Id, transaction.ToId);
            Assert.Equal(300, transaction.Amount);
            Assert.Equal(TransactionType.TransferAccountToAccount, transaction.Type);

            var log = await context.Logs.SingleAsync();
            Assert.Equal(ActionType.Transfer, log.Action);
            Assert.Equal(transaction.Id, log.TargetId);
        }

        [Fact]
        public async Task TransferAccountToAccount_InsufficientFunds_DoesNotChangeBalances()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var from = await _db.AddAccount(client.Id, bank.Id, 100);
            var to = await _db.AddAccount(client.Id, bank.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.TransferAccountToAccount(client.Id, from.Number!, to.Number!, 500));

            using var context = _db.CreateContext();
            Assert.Equal(100, (await context.Accounts.FindAsync(from.Id))!.Balance);
            Assert.Equal(0, (await context.Accounts.FindAsync(to.Id))!.Balance);
            Assert.Empty(await context.Transactions.ToListAsync());
        }

        [Fact]
        public async Task TransferAccountToAccount_FromForeignAccount_Throws()
        {
            var owner = await _db.AddClient("owner");
            var thief = await _db.AddClient("thief");
            var bank = await _db.AddBank();
            var from = await _db.AddAccount(owner.Id, bank.Id, 1000);
            var to = await _db.AddAccount(thief.Id, bank.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.TransferAccountToAccount(thief.Id, from.Number!, to.Number!, 100));
            Assert.Equal(1000, from.Balance);
        }

        [Fact]
        public async Task TransferAccountToAccount_SameAccount_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id, 1000);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.TransferAccountToAccount(client.Id, account.Number!, account.Number!, 100));
        }

        [Fact]
        public async Task TransferAccountToAccount_UnknownAccount_ThrowsNotFound()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var from = await _db.AddAccount(client.Id, bank.Id, 1000);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.TransferAccountToAccount(client.Id, from.Number!, "000000", 100));
        }

        [Fact]
        public async Task TransferAccountToAccount_FromBlockedAccount_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var from = await _db.AddAccount(client.Id, bank.Id, 1000);
            var to = await _db.AddAccount(client.Id, bank.Id);
            from.Block();
            await _db.UnitOfWork.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.TransferAccountToAccount(client.Id, from.Number!, to.Number!, 100));
        }

        [Fact]
        public async Task TransferAccountToAccount_NotApprovedClient_Throws()
        {
            var client = await _db.AddClient(approved: false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.TransferAccountToAccount(client.Id, "111111", "222222", 100));
        }

        [Fact]
        public async Task TransferAccountToDeposit_MovesMoney()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id, 1000);
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 100);

            await _service.TransferAccountToDeposit(client.Id, account.Number!, deposit.Number!, 250);

            using var context = _db.CreateContext();
            Assert.Equal(750, (await context.Accounts.SingleAsync()).Balance);
            Assert.Equal(350, (await context.Deposits.SingleAsync()).Balance);
            Assert.Equal(TransactionType.TransferAccountToDeposit, (await context.Transactions.SingleAsync()).Type);
        }

        [Fact]
        public async Task TransferDepositToAccount_OpenDeposit_MovesMoney()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 500);

            await _service.TransferDepositToAccount(client.Id, deposit.Number!, account.Number!, 200);

            using var context = _db.CreateContext();
            Assert.Equal(200, (await context.Accounts.SingleAsync()).Balance);
            Assert.Equal(300, (await context.Deposits.SingleAsync()).Balance);
            Assert.Equal(TransactionType.TransferDepositToAccount, (await context.Transactions.SingleAsync()).Type);
        }

        [Fact]
        public async Task TransferDepositToAccount_ClosedDepositType_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(client.Id, bank.Id);
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 500, DepositType.ClosedDeposit);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.TransferDepositToAccount(client.Id, deposit.Number!, account.Number!, 200));
        }

        [Fact]
        public async Task TransferDepositToAccount_ForeignDeposit_Throws()
        {
            var owner = await _db.AddClient("owner");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            var account = await _db.AddAccount(other.Id, bank.Id);
            var deposit = await _db.AddDeposit(owner.Id, bank.Id, 500);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.TransferDepositToAccount(other.Id, deposit.Number!, account.Number!, 100));
        }

        [Fact]
        public async Task TransferDepositToDeposit_MovesMoney()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var from = await _db.AddDeposit(client.Id, bank.Id, 500);
            var to = await _db.AddDeposit(client.Id, bank.Id, 100);

            await _service.TransferDepositToDeposit(client.Id, from.Number!, to.Number!, 150);

            using var context = _db.CreateContext();
            Assert.Equal(350, (await context.Deposits.FindAsync(from.Id))!.Balance);
            Assert.Equal(250, (await context.Deposits.FindAsync(to.Id))!.Balance);
            Assert.Equal(TransactionType.TransferDepositToDeposit, (await context.Transactions.SingleAsync()).Type);
        }

        [Fact]
        public async Task TransferDepositToDeposit_SameDeposit_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var deposit = await _db.AddDeposit(client.Id, bank.Id, 500);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.TransferDepositToDeposit(client.Id, deposit.Number!, deposit.Number!, 100));
        }

        public void Dispose() => _db.Dispose();
    }
}
