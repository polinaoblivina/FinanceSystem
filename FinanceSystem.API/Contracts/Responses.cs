using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Enums;

namespace FinanceSystem.API.Contracts
{
    public record LoginResponse(string Token, DateTime ExpiresAt, Guid UserId, string Login, string Role);

    public record ClientResponse(Guid Id, string Login, string FirstName, string LastName, bool IsApproved)
    {
        public static ClientResponse From(Client client) =>
            new(client.Id, client.Login!, client.FirstName!, client.LastName!, client.IsApprovedByManager);
    }

    public record BankResponse(Guid Id, string Name)
    {
        public static BankResponse From(Bank bank) => new(bank.Id, bank.Name!);
    }

    public record EnterpriseResponse(Guid Id, string Name)
    {
        public static EnterpriseResponse From(Enterprise enterprise) => new(enterprise.Id, enterprise.Name!);
    }

    public record EnterpriseEmployeesResponse(Guid Id, string Name, List<ClientResponse> Employees)
    {
        public static EnterpriseEmployeesResponse From(Enterprise enterprise, List<Client> employees) =>
            new(enterprise.Id, enterprise.Name!, employees.Select(ClientResponse.From).ToList());
    }

    public record AccountResponse(Guid Id, string Number, Guid BankId, decimal Balance, AccountStatus Status)
    {
        public static AccountResponse From(Account account) =>
            new(account.Id, account.Number!, account.BankId, account.Balance, account.Status);
    }

    public record DepositResponse(
        Guid Id,
        string Number,
        Guid BankId,
        decimal Balance,
        double InterestRate,
        DepositType Type,
        DepositStatus Status,
        DateTime LastInterestDate)
    {
        public static DepositResponse From(Deposit deposit) =>
            new(deposit.Id, deposit.Number!, deposit.BankId, deposit.Balance, deposit.InterestRate,
                deposit.Type, deposit.Status, deposit.LastInterestDate);
    }

    public record TransactionResponse(
        Guid Id,
        string Number,
        Guid FromId,
        Guid ToId,
        decimal Amount,
        TransactionType Type,
        DateTime Timestamp)
    {
        public static TransactionResponse From(Transaction transaction) =>
            new(transaction.Id, transaction.Number!, transaction.FromId, transaction.ToId,
                transaction.Amount, transaction.Type, transaction.Timestamp);
    }

    public record SalaryRequestResponse(Guid Id, string Number, Guid ClientId, Guid EnterpriseId, RequestStatus Status)
    {
        public static SalaryRequestResponse From(SalaryProjectRequest request) =>
            new(request.Id, request.Number!, request.ClientId, request.EnterpriseId, request.Status);
    }

    public record LogResponse(
        Guid Id,
        string Number,
        Guid UserId,
        ActionType Action,
        Guid? TargetId,
        LogTargetType? TargetType,
        DateTime Timestamp,
        string? FromInfo,
        string? ToInfo,
        decimal? Amount)
    {
        public static LogResponse From(SystemLog log) =>
            new(log.Id, log.Number!, log.UserId, log.Action, log.TargetId, log.TargetType,
                log.Timestamp, log.FromInfo, log.ToInfo, log.Amount);
    }
}
