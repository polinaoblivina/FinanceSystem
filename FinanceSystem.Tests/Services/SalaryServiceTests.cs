using FinanceSystem.Application.Exceptions;
using FinanceSystem.Application.Services.ClientServices;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Tests.Services
{
    public class SalaryServiceTests : IDisposable
    {
        private readonly TestDatabase _db = new();
        private readonly SalaryService _service;

        public SalaryServiceTests()
        {
            _service = new SalaryService(_db.UnitOfWork);
        }

        private async Task Employ(Guid clientId, Guid enterpriseId)
        {
            await _db.UnitOfWork.Employments.AddAsync(Employment.CreateNew(clientId, enterpriseId));
            await _db.UnitOfWork.SaveChangesAsync();
        }

        private async Task<SalaryProjectRequest> AddRequest(Guid clientId, Guid enterpriseId, bool approve)
        {
            var request = SalaryProjectRequest.CreateNew(clientId, enterpriseId);
            if (approve)
                request.Approve();
            await _db.UnitOfWork.SalaryRequests.AddAsync(request);
            await _db.UnitOfWork.SaveChangesAsync();
            return request;
        }

        [Fact]
        public async Task RequestSalaryProject_EmployedClient_CreatesPendingRequest()
        {
            var client = await _db.AddClient();
            var enterprise = await _db.AddEnterprise("Enterprise1");
            await Employ(client.Id, enterprise.Id);

            await _service.RequestSalaryProject(client.Id, "Enterprise1");

            using var context = _db.CreateContext();
            var request = await context.SalaryProjectRequests.SingleAsync();
            Assert.Equal(client.Id, request.ClientId);
            Assert.Equal(enterprise.Id, request.EnterpriseId);
            Assert.Equal(RequestStatus.Pending, request.Status);
        }

        [Fact]
        public async Task RequestSalaryProject_NotEmployed_Throws()
        {
            var client = await _db.AddClient();
            await _db.AddEnterprise("Enterprise1");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RequestSalaryProject(client.Id, "Enterprise1"));
        }

        [Fact]
        public async Task RequestSalaryProject_Duplicate_Throws()
        {
            var client = await _db.AddClient();
            var enterprise = await _db.AddEnterprise("Enterprise1");
            await Employ(client.Id, enterprise.Id);
            await _service.RequestSalaryProject(client.Id, "Enterprise1");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RequestSalaryProject(client.Id, "Enterprise1"));
        }

        [Fact]
        public async Task RequestSalaryProject_UnknownEnterprise_ThrowsNotFound()
        {
            var client = await _db.AddClient();

            await Assert.ThrowsAsync<NotFoundException>(() => _service.RequestSalaryProject(client.Id, "Nope"));
        }

        [Fact]
        public async Task ReceiveSalary_AcceptedRequest_IncreasesBalance()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var enterprise = await _db.AddEnterprise("Enterprise1");
            var account = await _db.AddAccount(client.Id, bank.Id);
            await Employ(client.Id, enterprise.Id);
            await AddRequest(client.Id, enterprise.Id, approve: true);

            await _service.ReceiveSalary(client.Id, "Enterprise1", account.Number!, 1500);

            using var context = _db.CreateContext();
            Assert.Equal(1500, (await context.Accounts.SingleAsync()).Balance);

            var transaction = await context.Transactions.SingleAsync();
            Assert.Equal(enterprise.Id, transaction.FromId);
            Assert.Equal(TransactionType.SalaryPayment, transaction.Type);

            var log = await context.Logs.SingleAsync();
            Assert.Equal(ActionType.ReceiveSalary, log.Action);
            Assert.Equal(transaction.Id, log.TargetId);
        }

        [Fact]
        public async Task ReceiveSalary_PendingRequest_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var enterprise = await _db.AddEnterprise("Enterprise1");
            var account = await _db.AddAccount(client.Id, bank.Id);
            await Employ(client.Id, enterprise.Id);
            await AddRequest(client.Id, enterprise.Id, approve: false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ReceiveSalary(client.Id, "Enterprise1", account.Number!, 1500));
            Assert.Equal(0, account.Balance);
        }

        [Fact]
        public async Task ReceiveSalary_WithoutRequest_Throws()
        {
            var client = await _db.AddClient();
            var bank = await _db.AddBank();
            var enterprise = await _db.AddEnterprise("Enterprise1");
            var account = await _db.AddAccount(client.Id, bank.Id);
            await Employ(client.Id, enterprise.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ReceiveSalary(client.Id, "Enterprise1", account.Number!, 1500));
        }

        [Fact]
        public async Task ReceiveSalary_ForeignAccount_Throws()
        {
            var client = await _db.AddClient("client");
            var other = await _db.AddClient("other");
            var bank = await _db.AddBank();
            var enterprise = await _db.AddEnterprise("Enterprise1");
            var account = await _db.AddAccount(other.Id, bank.Id);
            await Employ(client.Id, enterprise.Id);
            await AddRequest(client.Id, enterprise.Id, approve: true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ReceiveSalary(client.Id, "Enterprise1", account.Number!, 1500));
        }

        public void Dispose() => _db.Dispose();
    }
}
