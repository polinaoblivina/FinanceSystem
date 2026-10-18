using FInanceSystem.Domain.Entities;

namespace FinanceSystem.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Client_CreateNew_IsNotApproved()
        {
            var client = Client.CreateNew("login", "hash", "Ivan", "Ivanov");

            Assert.False(client.IsApprovedByManager);
            Assert.NotEqual(Guid.Empty, client.Id);
        }

        [Fact]
        public void Client_ApproveAndRevoke_ChangeApproval()
        {
            var client = Client.CreateNew("login", "hash", "Ivan", "Ivanov");

            client.Approve();
            Assert.True(client.IsApprovedByManager);
            Assert.Throws<InvalidOperationException>(() => client.Approve());

            client.RevokeApproval();
            Assert.False(client.IsApprovedByManager);
            Assert.Throws<InvalidOperationException>(() => client.RevokeApproval());
        }

        [Theory]
        [InlineData("", "hash", "Ivan", "Ivanov")]
        [InlineData("login", " ", "Ivan", "Ivanov")]
        [InlineData("login", "hash", "", "Ivanov")]
        [InlineData("login", "hash", "Ivan", null)]
        public void User_WithMissingFields_Throws(string login, string password, string firstName, string? lastName)
        {
            Assert.Throws<ArgumentException>(() => Client.CreateNew(login, password, firstName, lastName!));
            Assert.Throws<ArgumentException>(() => Admin.CreateNew(login, password, firstName, lastName!));
            Assert.Throws<ArgumentException>(() => Manager.CreateNew(login, password, firstName, lastName!));
        }
    }
}
