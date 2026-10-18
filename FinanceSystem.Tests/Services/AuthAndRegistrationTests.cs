using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.Common;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Tests.Services
{
    public class ClientRegistrationServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly ClientRegistrationService _service;

        public ClientRegistrationServiceTests()
        {
            _service = new ClientRegistrationService(_db.UnitOfWork, _db.PasswordHasher);
        }

        [Fact]
        public async Task Register_CreatesNotApprovedClientWithHashedPassword()
        {
            var client = await _service.Register("ivan", "secret", "Ivan", "Ivanov");

            using var context = _db.CreateContext();
            var saved = await context.Users.OfType<Client>().SingleAsync();
            Assert.Equal(client.Id, saved.Id);
            Assert.False(saved.IsApprovedByManager);
            Assert.Equal(_db.PasswordHasher.Hash("secret"), saved.Password);
            Assert.NotEqual("secret", saved.Password);
        }

        [Fact]
        public async Task Register_WritesCreateLog()
        {
            var client = await _service.Register("ivan", "secret", "Ivan", "Ivanov");

            using var context = _db.CreateContext();
            var log = await context.Logs.SingleAsync();
            Assert.Equal(client.Id, log.UserId);
            Assert.Equal(ActionType.Create, log.Action);
        }

        [Fact]
        public async Task Register_WithExistingLogin_Throws()
        {
            await _service.Register("ivan", "secret", "Ivan", "Ivanov");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Register("ivan", "other", "Petr", "Petrov"));
        }

        [Fact]
        public async Task Register_WithEmptyPassword_Throws()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.Register("ivan", " ", "Ivan", "Ivanov"));
        }

        public void Dispose() => _db.Dispose();
    }

    public class AuthServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _service = new AuthService(_db.UnitOfWork, _db.PasswordHasher);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsUser()
        {
            var client = await _db.AddClient("ivan");

            var user = await _service.Login("ivan", "parol");

            Assert.Equal(client.Id, user.Id);
        }

        [Fact]
        public async Task Login_WithWrongPassword_Throws()
        {
            await _db.AddClient("ivan");

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Login("ivan", "wrong"));
        }

        [Fact]
        public async Task Login_UnknownUser_Throws()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Login("nobody", "parol"));
        }

        [Theory]
        [InlineData("", "parol")]
        [InlineData("ivan", "")]
        public async Task Login_WithEmptyCredentials_Throws(string login, string password)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.Login(login, password));
        }

        public void Dispose() => _db.Dispose();
    }

    public class SeedServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly SeedService _service;

        public SeedServiceTests()
        {
            _service = new SeedService(_db.UnitOfWork, _db.PasswordHasher);
        }

        [Fact]
        public async Task SeedAllData_CreatesBanksEnterprisesAndStaff()
        {
            await _service.SeedAllData();

            using var context = _db.CreateContext();
            Assert.Equal(2, await context.Banks.CountAsync());
            Assert.Equal(2, await context.Enterprises.CountAsync());
            Assert.Single(await context.Users.OfType<Admin>().ToListAsync());
            Assert.Single(await context.Users.OfType<Manager>().ToListAsync());
        }

        [Fact]
        public async Task SeedAllData_StoresHashedPasswords()
        {
            await _service.SeedAllData();

            var admin = await _db.UnitOfWork.Users.GetByLoginAsync("admin");
            Assert.NotNull(admin);
            Assert.True(_db.PasswordHasher.Verify("adminparol", admin.Password!));
        }

        [Fact]
        public async Task SeedAllData_CalledTwice_DoesNotDuplicate()
        {
            await _service.SeedAllData();
            await _service.SeedAllData();

            using var context = _db.CreateContext();
            Assert.Equal(2, await context.Banks.CountAsync());
            Assert.Equal(2, await context.Enterprises.CountAsync());
            Assert.Equal(2, await context.Users.CountAsync());
        }

        public void Dispose() => _db.Dispose();
    }
}
