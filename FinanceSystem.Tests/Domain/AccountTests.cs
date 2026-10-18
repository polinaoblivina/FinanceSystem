using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;

namespace FinanceSystem.Tests.Domain
{
    public class AccountTests
    {
        private static Account NewAccount(decimal balance = 0, AccountStatus status = AccountStatus.Active)
            => Account.Restore(Guid.NewGuid(), "123456", Guid.NewGuid(), Guid.NewGuid(), balance, status);

        [Fact]
        public void CreateNew_CreatesActiveAccountWithZeroBalance()
        {
            var account = Account.CreateNew(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(AccountStatus.Active, account.Status);
            Assert.Equal(0, account.Balance);
            Assert.False(string.IsNullOrWhiteSpace(account.Number));
        }

        [Fact]
        public void CreateNew_WithEmptyClientId_Throws()
        {
            Assert.Throws<ArgumentException>(() => Account.CreateNew(Guid.Empty, Guid.NewGuid()));
        }

        [Fact]
        public void Restore_WithNegativeBalance_Throws()
        {
            Assert.Throws<ArgumentException>(() => NewAccount(balance: -1));
        }

        [Fact]
        public void IncreaseBalance_AddsAmount()
        {
            var account = NewAccount(100);

            account.IncreaseBalance(50);

            Assert.Equal(150, account.Balance);
        }

        [Fact]
        public void IncreaseBalance_OnBlockedAccount_IsAllowed()
        {
            var account = NewAccount(100, AccountStatus.Blocked);

            account.IncreaseBalance(50);

            Assert.Equal(150, account.Balance);
        }

        [Fact]
        public void IncreaseBalance_OnClosedAccount_Throws()
        {
            var account = NewAccount(0, AccountStatus.Closed);

            Assert.Throws<InvalidOperationException>(() => account.IncreaseBalance(50));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void IncreaseBalance_WithNonPositiveAmount_Throws(decimal amount)
        {
            var account = NewAccount(100);

            Assert.Throws<ArgumentException>(() => account.IncreaseBalance(amount));
        }

        [Fact]
        public void DecreaseBalance_SubtractsAmount()
        {
            var account = NewAccount(100);

            account.DecreaseBalance(30);

            Assert.Equal(70, account.Balance);
        }

        [Fact]
        public void DecreaseBalance_MoreThanBalance_Throws()
        {
            var account = NewAccount(100);

            Assert.Throws<InvalidOperationException>(() => account.DecreaseBalance(101));
            Assert.Equal(100, account.Balance);
        }

        [Fact]
        public void DecreaseBalance_OnBlockedAccount_Throws()
        {
            var account = NewAccount(100, AccountStatus.Blocked);

            Assert.Throws<InvalidOperationException>(() => account.DecreaseBalance(10));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void DecreaseBalance_WithNonPositiveAmount_Throws(decimal amount)
        {
            var account = NewAccount(100);

            Assert.Throws<ArgumentException>(() => account.DecreaseBalance(amount));
        }

        [Fact]
        public void Close_EmptyActiveAccount_SetsClosed()
        {
            var account = NewAccount();

            account.Close();

            Assert.Equal(AccountStatus.Closed, account.Status);
        }

        [Fact]
        public void Close_NonEmptyAccount_Throws()
        {
            var account = NewAccount(1);

            Assert.Throws<InvalidOperationException>(() => account.Close());
        }

        [Fact]
        public void Close_BlockedAccount_Throws()
        {
            var account = NewAccount(0, AccountStatus.Blocked);

            Assert.Throws<InvalidOperationException>(() => account.Close());
        }

        [Fact]
        public void BlockAndUnblock_ChangeStatus()
        {
            var account = NewAccount();

            account.Block();
            Assert.Equal(AccountStatus.Blocked, account.Status);

            account.Unblock();
            Assert.Equal(AccountStatus.Active, account.Status);
        }

        [Fact]
        public void Block_NonActiveAccount_Throws()
        {
            var account = NewAccount(0, AccountStatus.Closed);

            Assert.Throws<InvalidOperationException>(() => account.Block());
        }

        [Fact]
        public void Unblock_ActiveAccount_Throws()
        {
            var account = NewAccount();

            Assert.Throws<InvalidOperationException>(() => account.Unblock());
        }

        [Fact]
        public void Reopen_ClosedAccount_SetsActive()
        {
            var account = NewAccount(0, AccountStatus.Closed);

            account.Reopen();

            Assert.Equal(AccountStatus.Active, account.Status);
        }

        [Fact]
        public void Reopen_ActiveAccount_Throws()
        {
            var account = NewAccount();

            Assert.Throws<InvalidOperationException>(() => account.Reopen());
        }
    }
}
