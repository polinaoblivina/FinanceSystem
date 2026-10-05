using FInanceSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceSystem.API.Contracts
{
    public record LoginRequest(
        [Required] string Login,
        [Required] string Password);

    public record RegisterRequest(
        [Required, StringLength(50, MinimumLength = 3)] string Login,
        [Required, StringLength(100, MinimumLength = 6)] string Password,
        [Required, StringLength(50)] string FirstName,
        [Required, StringLength(50)] string LastName);

    public record OpenAccountRequest(
        [Required] string BankName);

    public record AccountNumberRequest(
        [Required] string AccountNumber);

    public record DepositNumberRequest(
        [Required] string DepositNumber);

    public record CreateDepositRequest(
        [Required] string AccountNumber,
        [Range(0.01, 1_000_000_000)] decimal Amount,
        [Range(0.01, 99.99)] double Rate,
        DepositType Type);

    public record CloseDepositRequest(
        [Required] string DepositNumber,
        [Required] string AccountNumber);

    public record TransferRequest(
        [Required] string From,
        [Required] string To,
        [Range(0.01, 1_000_000_000)] decimal Amount);

    public record EnterpriseRequest(
        [Required] string EnterpriseName);

    public record ReceiveSalaryRequest(
        [Required] string EnterpriseName,
        [Required] string AccountNumber,
        [Range(0.01, 1_000_000_000)] decimal Amount);

    public record ClientLoginRequest(
        [Required] string ClientLogin);

    public record ClientEnterpriseRequest(
        [Required] string ClientLogin,
        [Required] string EnterpriseName);

    public record SalaryRequestNumberRequest(
        [Required] string RequestNumber);

    public record RevertActionRequest(
        [Required] string LogNumber);
}
