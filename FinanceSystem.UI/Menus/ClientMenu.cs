using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.Common;
using FinanceSystem.Infrastructure;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace FinanceSystem.UI.Menus
{
    public class ClientMenu
    {
        public static async Task Show(ServiceProvider serviceProvider, Client? client = null)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            var accountService = serviceProvider.GetRequiredService<AccountService>();
            var depositService = serviceProvider.GetRequiredService<DepositService>();
            var transferService = serviceProvider.GetRequiredService<TransferService>();
            var infoService = serviceProvider.GetRequiredService<InfoService>();
            var salaryService = serviceProvider.GetRequiredService<SalaryService>();

            if (client == null)
            {
                var authService = serviceProvider.GetRequiredService<AuthService>();

                Console.Write("Login: ");
                var login = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();

                try
                {
                    var user = await authService.Login(login, password);
                    client = user as Client;
                    if (client == null)
                    {
                        Console.WriteLine("\nThis user is not a client");
                        Thread.Sleep(2000);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Thread.Sleep(2000);
                    return;
                }
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Client: {client.Login}\n");
                Console.WriteLine($"Status: {(client.IsApprovedByManager ? "Approved" : "Pending approval")}\n");

                Console.WriteLine("1. Show banks");
                Console.WriteLine("2. Show enterprises");
                Console.WriteLine("3. Open account");
                Console.WriteLine("4. Close account");
                Console.WriteLine("5. Show my accounts");
                Console.WriteLine("6. Show account transactions");
                Console.WriteLine("7. Create deposit");
                Console.WriteLine("8. Close deposit");
                Console.WriteLine("9. Show my deposits");
                Console.WriteLine("10. Accrue interest on deposit");
                Console.WriteLine("11. Transfer account -> account");
                Console.WriteLine("12. Transfer account -> deposit");
                Console.WriteLine("13. Transfer deposit -> account");
                Console.WriteLine("14. Transfer deposit -> deposit");
                Console.WriteLine("15. Request salary project");
                Console.WriteLine("16. Receive salary");
                Console.WriteLine("0. Exit");

                Console.Write("\nSelect option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowBanks(infoService);
                        break;
                    case "2":
                        await ShowEnterprises(infoService);
                        break;
                    case "3":
                        await OpenAccount(accountService, client);
                        break;
                    case "4":
                        await CloseAccount(accountService, client);
                        break;
                    case "5":
                        await ShowAccounts(accountService, unitOfWork, client);
                        break;
                    case "6":
                        await ShowAccountTransactions(accountService, unitOfWork, client);
                        break;
                    case "7":
                        await CreateDeposit(depositService, client);
                        break;
                    case "8":
                        await CloseDeposit(depositService, client);
                        break;
                    case "9":
                        await ShowDeposits(depositService, unitOfWork, client);
                        break;
                    case "10":
                        await AccrueInterest(depositService, client);
                        break;
                    case "11":
                        await TransferAccountToAccount(transferService, client);
                        break;
                    case "12":
                        await TransferAccountToDeposit(transferService, client);
                        break;
                    case "13":
                        await TransferDepositToAccount(transferService, client);
                        break;
                    case "14":
                        await TransferDepositToDeposit(transferService, client);
                        break;
                    case "15":
                        await RequestSalaryProject(salaryService, client, infoService);
                        break;
                    case "16":
                        await ReceiveSalary(salaryService, client, infoService);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice!");
                        Thread.Sleep(1500);
                        break;
                }
            }
        }

        static async Task ShowBanks(InfoService infoService)
        {
            try
            {
                Console.WriteLine("\nBanks");
                var banks = await infoService.GetBanks();
                foreach (var bank in banks)
                {
                    Console.WriteLine($"-{bank.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task ShowEnterprises(InfoService infoService)
        {
            try
            {
                Console.WriteLine("\nEnterprises");
                var enterprises = await infoService.GetEnterprises();
                foreach (var enterprise in enterprises)
                {
                    Console.WriteLine($"-{enterprise.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task ShowAccounts(AccountService accountService, IUnitOfWork unitOfWork, Client client)
        {
            try
            {
                Console.WriteLine("\nAccounts");

                var accounts = await accountService.GetClientAccounts(client.Id);

                if (accounts.Count == 0)
                {
                    Console.WriteLine("Accounts not found.");
                }
                else
                {
                    foreach (var account in accounts)
                    {
                        var bank = await unitOfWork.Banks.GetByIdAsync(account.BankId);
                        var bankName = bank?.Name ?? "Unknown";

                        Console.WriteLine($"Account: {account.Number}");
                        Console.WriteLine($"-Bank: {bankName}");
                        Console.WriteLine($"-Balance: {account.Balance:C}");
                        Console.WriteLine($"-Status: {account.Status}");
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task OpenAccount(AccountService accountService, Client client)
        {
            try
            {
                Console.Write("Bank name: ");
                var bankName = Console.ReadLine();

                var account = await accountService.OpenAccount(client.Id, bankName);
                Console.WriteLine($"\nAccount opened!");
                Console.WriteLine($"Account number: {account.Number}");
                Console.WriteLine($"Balance: {account.Balance:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task CloseAccount(AccountService accountService, Client client)
        {
            try
            {
                Console.Write("Account Number: ");
                var accountNumber = Console.ReadLine();

                await accountService.CloseAccount(client.Id, accountNumber);
                Console.WriteLine($"\nAccount closed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task ShowAccountTransactions(AccountService accountService, IUnitOfWork unitOfWork, Client client)
        {
            try
            {
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();

                var transactions = await accountService.GetTransactions(client.Id, accountNumber);
                Console.WriteLine($"\nTransactions");

                if (transactions.Count == 0)
                {
                    Console.WriteLine("No transactions found.");
                }
                else
                {
                    foreach (var t in transactions)
                    {
                        string fromInfo = "Unknown";
                        if (t.FromId != Guid.Empty)
                        {
                            var fromAccount = await unitOfWork.Accounts.GetByIdAsync(t.FromId);
                            if (fromAccount != null)
                                fromInfo = $"Account {fromAccount.Number}";
                            else
                            {
                                var fromDeposit = await unitOfWork.Deposits.GetByIdAsync(t.FromId);
                                if (fromDeposit != null)
                                    fromInfo = $"Deposit {fromDeposit.Number}";
                                else
                                {
                                    var fromEnterprise = await unitOfWork.Enterprises.GetByIdAsync(t.FromId);
                                    if (fromEnterprise != null)
                                        fromInfo = $"Enterprise {fromEnterprise.Name}";
                                }
                            }
                        }

                        string toInfo = "Unknown";
                        if (t.ToId != Guid.Empty)
                        {
                            var toAccount = await unitOfWork.Accounts.GetByIdAsync(t.ToId);
                            if (toAccount != null)
                                toInfo = $"Account {toAccount.Number}";
                            else
                            {
                                var toDeposit = await unitOfWork.Deposits.GetByIdAsync(t.ToId);
                                if (toDeposit != null)
                                    toInfo = $"Deposit {toDeposit.Number}";
                            }
                        }

                        Console.WriteLine($"[{t.Timestamp:yyyy-MM-dd HH:mm}] {t.Type}");
                        Console.WriteLine($"From: {fromInfo}");
                        Console.WriteLine($"To: {toInfo}");
                        Console.WriteLine($"Amount: {t.Amount:C}");
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }
        static async Task CreateDeposit(DepositService depositService, Client client)
        {
            try
            {
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();
                Console.Write("Amount: ");
                var amount = decimal.Parse(Console.ReadLine());
                Console.Write("Interest rate (%): ");
                var rate = double.Parse(Console.ReadLine());
                Console.Write("Deposit type (OpenDeposit/ClosedDeposit): ");
                var type = Enum.Parse<FInanceSystem.Domain.Enums.DepositType>(Console.ReadLine());

                var deposit = await depositService.CreateDeposit(client.Id, accountNumber, amount, rate, type);
                Console.WriteLine($"\nDeposit created!");
                Console.WriteLine($"-Deposit number: {deposit.Number}");
                Console.WriteLine($"-Balance: {deposit.Balance:C}");
                Console.WriteLine($"-Interest rate: {deposit.InterestRate}%");
                Console.WriteLine($"-Type: {deposit.Type}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task CloseDeposit(DepositService depositService, Client client)
        {
            try
            {
                Console.Write("Deposit number: ");
                var depositNumber = Console.ReadLine();
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();

                await depositService.CloseDeposit(client.Id, depositNumber, accountNumber);
                Console.WriteLine($"\nDeposit closed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task ShowDeposits(DepositService depositService, IUnitOfWork unitOfWork, Client client)
        {
            try
            {
                Console.WriteLine("\nDeposites");

                var deposits = await depositService.GetClientDeposits(client.Id);

                if (deposits.Count == 0)
                {
                    Console.WriteLine("Deposits not found.");
                }
                else
                {
                    foreach (var deposit in deposits)
                    {
                        var bank = await unitOfWork.Banks.GetByIdAsync(deposit.BankId);
                        var bankName = bank?.Name ?? "Unknown";

                        Console.WriteLine($"Deposit: {deposit.Number}");
                        Console.WriteLine($"-Balance: {deposit.Balance:C}");
                        Console.WriteLine($"-Bank: {bankName}");
                        Console.WriteLine($"-Interest rate: {deposit.InterestRate}%");
                        Console.WriteLine($"-Type: {deposit.Type}");
                        Console.WriteLine($"-Status: {deposit.Status}");
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task AccrueInterest(DepositService depositService, Client client)
        {
            try
            {
                Console.Write("Deposit number: ");
                var depositNumber = Console.ReadLine();

                await depositService.AccrueInterest(client.Id, depositNumber);
                Console.WriteLine($"\nInterest accrued!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task TransferAccountToAccount(TransferService transferService, Client client)
        {
            try
            {
                Console.Write("From account number: ");
                var from = Console.ReadLine();
                Console.Write("To account number: ");
                var to = Console.ReadLine();
                Console.Write("Amount: ");
                var amount = decimal.Parse(Console.ReadLine());

                await transferService.TransferAccountToAccount(client.Id, from, to, amount);
                Console.WriteLine($"\nTransfer completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task TransferAccountToDeposit(TransferService transferService, Client client)
        {
            try
            {
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();
                Console.Write("Deposit number: ");
                var depositNumber = Console.ReadLine();
                Console.Write("Amount: ");
                var amount = decimal.Parse(Console.ReadLine());

                await transferService.TransferAccountToDeposit(client.Id, accountNumber, depositNumber, amount);
                Console.WriteLine($"\nTransfer completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task TransferDepositToAccount(TransferService transferService, Client client)
        {
            try
            {
                Console.Write("Deposit number: ");
                var depositNumber = Console.ReadLine();
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();
                Console.Write("Amount: ");
                var amount = decimal.Parse(Console.ReadLine());

                await transferService.TransferDepositToAccount(client.Id, depositNumber, accountNumber, amount);
                Console.WriteLine($"\nTransfer completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task TransferDepositToDeposit(TransferService transferService, Client client)
        {
            try
            {
                Console.Write("From deposit number: ");
                var from = Console.ReadLine();
                Console.Write("To deposit number: ");
                var to = Console.ReadLine();
                Console.Write("Amount: ");
                var amount = decimal.Parse(Console.ReadLine());

                await transferService.TransferDepositToDeposit(client.Id, from, to, amount);
                Console.WriteLine($"\nTransfer completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }
        static async Task RequestSalaryProject(SalaryService salaryService, Client client, InfoService infoService)
        {
            try
            {
                Console.Write("Enterprise name: ");
                var enterpriseName = Console.ReadLine();

                await salaryService.RequestSalaryProject(client.Id, enterpriseName);
                Console.WriteLine($"\nRequest sent!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task ReceiveSalary(SalaryService salaryService, Client client, InfoService infoService)
        {
            try
            {
                Console.Write("Enterprise name: ");
                var enterpriseName = Console.ReadLine();
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();
                Console.Write("Amount: ");
                var amount = decimal.Parse(Console.ReadLine());
            
                await salaryService.ReceiveSalary(client.Id, enterpriseName, accountNumber, amount);
                Console.WriteLine($"\nSalary received!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }
    }
}
