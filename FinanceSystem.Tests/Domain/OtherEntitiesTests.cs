using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;

namespace FinanceSystem.Tests.Domain
{
    public class SalaryProjectRequestTests
    {
        [Fact]
        public void CreateNew_IsPending()
        {
            var request = SalaryProjectRequest.CreateNew(Guid.NewGuid(), Guid.NewGuid());

            Assert.Equal(RequestStatus.Pending, request.Status);
        }

        [Fact]
        public void Approve_PendingRequest_SetsAccepted()
        {
            var request = SalaryProjectRequest.CreateNew(Guid.NewGuid(), Guid.NewGuid());

            request.Approve();

            Assert.Equal(RequestStatus.Accepted, request.Status);
            Assert.Throws<InvalidOperationException>(() => request.Approve());
            Assert.Throws<InvalidOperationException>(() => request.Reject());
        }

        [Fact]
        public void Reject_PendingRequest_SetsRejected()
        {
            var request = SalaryProjectRequest.CreateNew(Guid.NewGuid(), Guid.NewGuid());

            request.Reject();

            Assert.Equal(RequestStatus.Rejected, request.Status);
            Assert.Throws<InvalidOperationException>(() => request.Approve());
        }
    }

    public class TransactionTests
    {
        [Fact]
        public void CreateNew_SetsFields()
        {
            var from = Guid.NewGuid();
            var to = Guid.NewGuid();

            var transaction = Transaction.CreateNew(from, to, 100, TransactionType.TransferAccountToAccount);

            Assert.Equal(from, transaction.FromId);
            Assert.Equal(to, transaction.ToId);
            Assert.Equal(100, transaction.Amount);
            Assert.Equal(TransactionType.TransferAccountToAccount, transaction.Type);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void CreateNew_WithNonPositiveAmount_Throws(decimal amount)
        {
            Assert.Throws<ArgumentException>(() => Transaction.CreateNew(Guid.NewGuid(), Guid.NewGuid(), amount, TransactionType.SalaryPayment));
        }

        [Fact]
        public void CreateNew_WithEmptyIds_Throws()
        {
            Assert.Throws<ArgumentException>(() => Transaction.CreateNew(Guid.Empty, Guid.NewGuid(), 1, TransactionType.SalaryPayment));
            Assert.Throws<ArgumentException>(() => Transaction.CreateNew(Guid.NewGuid(), Guid.Empty, 1, TransactionType.SalaryPayment));
        }
    }

    public class SystemLogTests
    {
        [Fact]
        public void CreateTransferLog_StoresTransferInfo()
        {
            var userId = Guid.NewGuid();
            var targetId = Guid.NewGuid();

            var log = SystemLog.CreateTransferLog(userId, ActionType.Transfer, targetId, LogTargetType.Transaction, "Account 1", "Account 2", 50);

            Assert.Equal(userId, log.UserId);
            Assert.Equal(targetId, log.TargetId);
            Assert.Equal("Account 1", log.FromInfo);
            Assert.Equal("Account 2", log.ToInfo);
            Assert.Equal(50, log.Amount);
        }

        [Fact]
        public void CreateRevertLog_DescribesOriginalAction()
        {
            var log = SystemLog.CreateRevertLog(Guid.NewGuid(), "12345678", "Transfer");

            Assert.Equal(ActionType.Revert, log.Action);
            Assert.Contains("Transfer", log.ToInfo);
            Assert.Contains("12345678", log.ToInfo);
        }

        [Fact]
        public void Create_WithEmptyUserId_Throws()
        {
            Assert.Throws<ArgumentException>(() => SystemLog.Create(Guid.Empty, ActionType.Create));
        }
    }

    public class BankAndEnterpriseTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNew_WithEmptyName_Throws(string name)
        {
            Assert.Throws<ArgumentException>(() => Bank.CreateNew(name));
            Assert.Throws<ArgumentException>(() => Enterprise.CreateNew(name));
        }

        [Fact]
        public void Employment_WithEmptyIds_Throws()
        {
            Assert.Throws<ArgumentException>(() => Employment.CreateNew(Guid.Empty, Guid.NewGuid()));
            Assert.Throws<ArgumentException>(() => Employment.CreateNew(Guid.NewGuid(), Guid.Empty));
        }
    }
}
