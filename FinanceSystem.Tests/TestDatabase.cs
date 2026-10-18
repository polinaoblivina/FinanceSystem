using FinanceSystem.Application.Interfaces;
using FinanceSystem.Infrastructure;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using FInanceSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Tests
{
    public sealed class TestDatabase : IDisposable
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        public FinanceDbContext Context { get; }
        public IUnitOfWork UnitOfWork { get; }
        public IPasswordHasher PasswordHasher { get; } = new FakePasswordHasher();

        public TestDatabase()
        {
            Context = CreateContext();
            UnitOfWork = new UnitOfWork(Context);
        }

        public FinanceDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<FinanceDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;
            return new FinanceDbContext(options);
        }

        public async Task<Client> AddClient(string login = "client", bool approved = true)
        {
            var client = Client.Restore(Guid.NewGuid(), login, PasswordHasher.Hash("parol"), "Ivan", "Ivanov", approved);
            await UnitOfWork.Users.AddAsync(client);
            await UnitOfWork.SaveChangesAsync();
            return client;
        }

        public async Task<Bank> AddBank(string name = "Bank1")
        {
            var bank = Bank.CreateNew(name);
            await UnitOfWork.Banks.AddAsync(bank);
            await UnitOfWork.SaveChangesAsync();
            return bank;
        }

        public async Task<Enterprise> AddEnterprise(string name = "Enterprise1")
        {
            var enterprise = Enterprise.CreateNew(name);
            await UnitOfWork.Enterprises.AddAsync(enterprise);
            await UnitOfWork.SaveChangesAsync();
            return enterprise;
        }

        public async Task<Account> AddAccount(Guid clientId, Guid bankId, decimal balance = 0)
        {
            var account = Account.CreateNew(clientId, bankId);
            if (balance > 0)
                account.IncreaseBalance(balance);
            await UnitOfWork.Accounts.AddAsync(account);
            await UnitOfWork.SaveChangesAsync();
            return account;
        }

        public async Task<Deposit> AddDeposit(Guid clientId, Guid bankId, decimal balance, DepositType type = DepositType.OpenDeposit)
        {
            var deposit = Deposit.CreateNew(clientId, bankId, balance, 0.1, type);
            await UnitOfWork.Deposits.AddAsync(deposit);
            await UnitOfWork.SaveChangesAsync();
            return deposit;
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }

    public class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => $"hashed:{password}";

        public bool Verify(string password, string passwordHash) => passwordHash == Hash(password);
    }
}
