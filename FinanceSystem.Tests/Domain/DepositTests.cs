using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;

namespace FinanceSystem.Tests.Domain
{
    public class DepositTests
    {
        private static Deposit NewDeposit(
            decimal balance = 1000,
            double rate = 0.1,
            DepositType type = DepositType.OpenDeposit,
            DepositStatus status = DepositStatus.Active,
            DateTime? lastInterestDate = null)
            => Deposit.Restore(Guid.NewGuid(), "654321", Guid.NewGuid(), Guid.NewGuid(), balance, rate, type, status, lastInterestDate ?? DateTime.UtcNow);

        [Fact]
        public void CreateNew_CreatesActiveDeposit()
        {
            var deposit = Deposit.CreateNew(Guid.NewGuid(), Guid.NewGuid(), 500, 0.1, DepositType.ClosedDeposit);

            Assert.Equal(DepositStatus.Active, deposit.Status);
            Assert.Equal(500, deposit.Balance);
            Assert.Equal(DepositType.ClosedDeposit, deposit.Type);
            Assert.False(string.IsNullOrWhiteSpace(deposit.Number));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(150)]
        public void Restore_WithInvalidInterestRate_Throws(double rate)
        {
            Assert.Throws<ArgumentException>(() => NewDeposit(rate: rate));
        }

        [Fact]
        public void Restore_WithNegativeBalance_Throws()
        {
            Assert.Throws<ArgumentException>(() => NewDeposit(balance: -1));
        }

        [Fact]
        public void DecreaseBalance_OpenDeposit_SubtractsAmount()
        {
            var deposit = NewDeposit(1000);

            deposit.DecreaseBalance(400);

            Assert.Equal(600, deposit.Balance);
        }

        [Fact]
        public void DecreaseBalance_ClosedDepositType_Throws()
        {
            var deposit = NewDeposit(1000, type: DepositType.ClosedDeposit);

            Assert.Throws<InvalidOperationException>(() => deposit.DecreaseBalance(100));
        }

        [Fact]
        public void DecreaseBalance_MoreThanBalance_Throws()
        {
            var deposit = NewDeposit(100);

            Assert.Throws<InvalidOperationException>(() => deposit.DecreaseBalance(101));
        }

        [Fact]
        public void DecreaseBalance_BlockedDeposit_Throws()
        {
            var deposit = NewDeposit(100, status: DepositStatus.Blocked);

            Assert.Throws<InvalidOperationException>(() => deposit.DecreaseBalance(10));
        }

        [Fact]
        public void IncreaseBalance_ActiveOrBlockedDeposit_AddsAmount()
        {
            var active = NewDeposit(100);
            var blocked = NewDeposit(100, status: DepositStatus.Blocked);

            active.IncreaseBalance(50);
            blocked.IncreaseBalance(50);

            Assert.Equal(150, active.Balance);
            Assert.Equal(150, blocked.Balance);
        }

        [Fact]
        public void IncreaseBalance_ClosedDeposit_Throws()
        {
            var deposit = NewDeposit(0, status: DepositStatus.Closed);

            Assert.Throws<InvalidOperationException>(() => deposit.IncreaseBalance(10));
        }

        [Fact]
        public void IncreaseBalance_NonPositiveAmount_Throws()
        {
            var deposit = NewDeposit();

            Assert.Throws<ArgumentException>(() => deposit.IncreaseBalance(0));
        }

        [Fact]
        public void AccrueInterest_AddsDailyInterestForElapsedDays()
        {
            var deposit = NewDeposit(balance: 3650, rate: 0.1, lastInterestDate: DateTime.UtcNow.AddDays(-10));

            deposit.AccrueInterest();

            // 3650 * 0.1 / 365 * 10 = 10
            Assert.Equal(3660, decimal.Round(deposit.Balance, 2));
            Assert.True(deposit.LastInterestDate > DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public void AccrueInterest_SameDay_DoesNotChangeBalance()
        {
            var deposit = NewDeposit(balance: 1000);

            deposit.AccrueInterest();

            Assert.Equal(1000, deposit.Balance);
        }

        [Fact]
        public void AccrueInterest_ZeroBalance_Throws()
        {
            var deposit = NewDeposit(balance: 0);

            Assert.Throws<InvalidOperationException>(() => deposit.AccrueInterest());
        }

        [Fact]
        public void AccrueInterest_BlockedDeposit_Throws()
        {
            var deposit = NewDeposit(status: DepositStatus.Blocked);

            Assert.Throws<InvalidOperationException>(() => deposit.AccrueInterest());
        }

        [Fact]
        public void Close_SetsClosedAndZeroBalance()
        {
            var deposit = NewDeposit(1000);

            deposit.Close();

            Assert.Equal(DepositStatus.Closed, deposit.Status);
            Assert.Equal(0, deposit.Balance);
        }

        [Fact]
        public void Close_WithoutInterest_DoesNotRequirePositiveBalance()
        {
            var deposit = NewDeposit(0);

            deposit.Close(accrueInterest: false);

            Assert.Equal(DepositStatus.Closed, deposit.Status);
        }

        [Fact]
        public void Close_NonActiveDeposit_Throws()
        {
            var deposit = NewDeposit(status: DepositStatus.Blocked);

            Assert.Throws<InvalidOperationException>(() => deposit.Close());
        }

        [Fact]
        public void BlockUnblockReopen_FollowStatusRules()
        {
            var deposit = NewDeposit();

            deposit.Block();
            Assert.Equal(DepositStatus.Blocked, deposit.Status);
            Assert.Throws<InvalidOperationException>(() => deposit.Block());

            deposit.Unblock();
            Assert.Equal(DepositStatus.Active, deposit.Status);
            Assert.Throws<InvalidOperationException>(() => deposit.Unblock());
            Assert.Throws<InvalidOperationException>(() => deposit.Reopen());

            deposit.Close(false);
            deposit.Reopen();
            Assert.Equal(DepositStatus.Active, deposit.Status);
        }
    }
}
