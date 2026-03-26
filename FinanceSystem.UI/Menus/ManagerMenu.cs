using FinanceSystem.Application.Services.Common;
using FinanceSystem.Application.Services.ManagerServices;
using FinanceSystem.Infrastructure;
using FInanceSystem.Domain.Entities;
using FInanceSystem.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace FinanceSystem.UI.Menus
{
    public class ManagerMenu
    {
        public static async Task Show(ServiceProvider serviceProvider, Manager? manager = null)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            var accountControlService = serviceProvider.GetRequiredService<AccountControlService>();
            var clientManagementService = serviceProvider.GetRequiredService<ClientManagementService>();
            var enterpriseManagementService = serviceProvider.GetRequiredService<EnterpriseManagementService>();
            var salaryRequestService = serviceProvider.GetRequiredService<SalaryRequestService>();

            if (manager == null)
            {
                var authService = serviceProvider.GetRequiredService<AuthService>();

                Console.Write("Login: ");
                var login = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();

                try
                {
                    var user = await authService.Login(login, password);
                    manager = user as Manager;
                    if (manager == null)
                    {
                        Console.WriteLine("\nThis user is not a manager!");
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
                Console.WriteLine($"Manager: {manager.Login}\n");

                Console.WriteLine("1. Show all clients");
                Console.WriteLine("2. Approve client");
                Console.WriteLine("3. Add client to enterprise");
                Console.WriteLine("4. Remove client from enterprise");
                Console.WriteLine("5. Show enterprises with employees");
                Console.WriteLine("6. Show pending salary requests");
                Console.WriteLine("7. Approve salary request");
                Console.WriteLine("8. Reject salary request");
                Console.WriteLine("9. Block account");
                Console.WriteLine("10. Unblock account");
                Console.WriteLine("11. Show account transactions");
                Console.WriteLine("12. Show client accounts");
                Console.WriteLine("13. Show client deposits");
                Console.WriteLine("14. Block deposit");
                Console.WriteLine("15. Unblock deposit");
                Console.WriteLine("0. Exit");

                Console.Write("\nSelect option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowAllClients(clientManagementService);
                        break;
                    case "2":
                        await ApproveClient(clientManagementService, manager);
                        break;
                    case "3":
                        await AddClientToEnterprise(enterpriseManagementService, manager);
                        break;
                    case "4":
                        await RemoveClientFromEnterprise(enterpriseManagementService, manager);
                        break;
                    case "5":
                        await ShowEnterprisesWithEmployees(enterpriseManagementService);
                        break;
                    case "6":
                        await ShowPendingSalaryRequests(salaryRequestService, unitOfWork);
                        break;
                    case "7":
                        await ApproveSalaryRequest(salaryRequestService, manager);
                        break;
                    case "8":
                        await RejectSalaryRequest(salaryRequestService, manager);
                        break;
                    case "9":
                        await BlockAccount(accountControlService, manager);
                        break;
                    case "10":
                        await UnblockAccount(accountControlService, manager);
                        break;
                    case "11":
                        await ShowAccountTransactions(accountControlService, unitOfWork);
                        break;
                    case "12":
                        await ShowClientAccounts(accountControlService);
                        break;
                    case "13":
                        await ShowClientDeposits(accountControlService);
                        break;
                    case "14":
                        await BlockDeposit(accountControlService, manager);
                        break;
                    case "15":
                        await UnblockDeposit(accountControlService, manager);
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

        static async Task ShowAllClients(ClientManagementService clientManagementService)
        {
            try
            {
                Console.WriteLine("\nClients\n");
                var clients = await clientManagementService.GetAllClients();

                if (clients.Count == 0)
                {
                    Console.WriteLine("Clients not found.");
                }
                else
                {
                    foreach (var client in clients)
                    {
                        Console.WriteLine($"Client: {client.Login}");
                        Console.WriteLine($"-Name: {client.FirstName} {client.LastName}");
                        Console.WriteLine($"-Status: {(client.IsApprovedByManager ? "Approved" : "Pending approval")}");
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

        static async Task ApproveClient(ClientManagementService clientManagementService, Manager manager)
        {
            try
            {
                Console.Write("Client login: ");
                var clientLogin = Console.ReadLine();

                await clientManagementService.ApproveClient(manager.Id, clientLogin);
                Console.WriteLine($"\nClient '{clientLogin}' approved");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task AddClientToEnterprise(EnterpriseManagementService enterpriseManagementService, Manager manager)
        {
            try
            {
                Console.Write("Client login: ");
                var clientLogin = Console.ReadLine();
                Console.Write("Enterprise name: ");
                var enterpriseName = Console.ReadLine();

                await enterpriseManagementService.AddClientToEnterprise(manager.Id, clientLogin, enterpriseName);
                Console.WriteLine($"\nClient '{clientLogin}' added to '{enterpriseName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task RemoveClientFromEnterprise(EnterpriseManagementService enterpriseManagementService, Manager manager)
        {
            try
            {
                Console.Write("Client login: ");
                var clientLogin = Console.ReadLine();
                Console.Write("Enterprise name: ");
                var enterpriseName = Console.ReadLine();

                await enterpriseManagementService.RemoveClientFromEnterprise(manager.Id, clientLogin, enterpriseName);
                Console.WriteLine($"\nClient '{clientLogin}' removed from '{enterpriseName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task ShowEnterprisesWithEmployees(EnterpriseManagementService enterpriseManagementService)
        {
            try
            {
                Console.WriteLine("\nEnterprises with employees:");
                var data = await enterpriseManagementService.GetAllEnterprisesWithEmployees();

                if (data.Count == 0)
                {
                    Console.WriteLine("Enterprises not found.");
                }
                else
                {
                    foreach (var (enterprise, employees) in data)
                    {
                        Console.WriteLine($"Enterprise: {enterprise.Name}");
                        if (employees.Count == 0)
                        {
                            Console.WriteLine("No employees");
                        }
                        else
                        {
                            foreach (var emp in employees)
                            {
                                Console.WriteLine($"-{emp.Login} ({emp.FirstName} {emp.LastName})");
                            }
                        }
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

        static async Task ShowPendingSalaryRequests(SalaryRequestService salaryRequestService, IUnitOfWork unitOfWork)
        {
            try
            {
                Console.WriteLine("\nPending salary requests:");

                var requests = await salaryRequestService.GetPendingRequests();

                if (requests.Count == 0)
                {
                    Console.WriteLine("No pending salary requests.");
                }
                else
                {
                    foreach (var req in requests)
                    {
                        var client = await unitOfWork.Users.GetByIdAsync(req.ClientId);
                        var clientLogin = client?.Login ?? "Unknown";

                        var enterprise = await unitOfWork.Enterprises.GetByIdAsync(req.EnterpriseId);
                        var enterpriseName = enterprise?.Name ?? "Unknown";

                        Console.WriteLine($"Request: {req.Number}");
                        Console.WriteLine($"-Client: {clientLogin}");
                        Console.WriteLine($"-Enterprise: {enterpriseName}");
                        Console.WriteLine($"-Status: {req.Status}");
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

        static async Task ApproveSalaryRequest(SalaryRequestService salaryRequestService, Manager manager)
        {
            try
            {
                Console.Write("Request number: ");
                var requestNumber = Console.ReadLine();

                await salaryRequestService.ApproveSalaryRequest(manager.Id, requestNumber);
                Console.WriteLine($"\nSalary request '{requestNumber}' approved");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task RejectSalaryRequest(SalaryRequestService salaryRequestService, Manager manager)
        {
            try
            {
                Console.Write("Request number: ");
                var requestNumber = Console.ReadLine();

                await salaryRequestService.RejectSalaryRequest(manager.Id, requestNumber);
                Console.WriteLine($"\nSalary request '{requestNumber}' rejected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task BlockAccount(AccountControlService accountControlService, Manager manager)
        {
            try
            {
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();
                
                await accountControlService.BlockAccount(manager.Id, accountNumber);
                Console.WriteLine($"\nAccount '{accountNumber}' blocked");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task UnblockAccount(AccountControlService accountControlService, Manager manager)
        {
            try
            {
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();

                await accountControlService.UnblockAccount(manager.Id, accountNumber);
                Console.WriteLine($"\nAccount '{accountNumber}' unblocked");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task ShowAccountTransactions(AccountControlService accountControlService, IUnitOfWork unitOfWork)
        {
            try
            {
                Console.Write("Account number: ");
                var accountNumber = Console.ReadLine();

                var transactions = await accountControlService.GetAccountTransactions(accountNumber);
                Console.WriteLine($"\nTransactions\n");

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
                        Console.WriteLine($"-From: {fromInfo}");
                        Console.WriteLine($"-To: {toInfo}");
                        Console.WriteLine($"-Amount: {t.Amount:C}");
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static async Task ShowClientAccounts(AccountControlService accountControlService)
        {
            try
            {
                Console.Write("Client login: ");
                var clientLogin = Console.ReadLine();
                var accounts = await accountControlService.GetClientAccounts(clientLogin);
                Console.WriteLine($"\nAccounts of {clientLogin}\n");

                if (accounts.Count == 0)
                {
                    Console.WriteLine("Accounts not found.");
                }
                else
                {
                    foreach (var account in accounts)
                    {
                        Console.WriteLine($"Account: {account.Number}");
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

        static async Task ShowClientDeposits(AccountControlService accountControlService)
        {
            try
            {
                Console.Write("Client login: ");
                var clientLogin = Console.ReadLine();

                var deposits = await accountControlService.GetClientDeposits(clientLogin);
                Console.WriteLine($"\nDeposits of {clientLogin}\n");

                if (deposits.Count == 0)
                {
                    Console.WriteLine("Deposits not found.");
                }
                else
                {
                    foreach (var deposit in deposits)
                    {
                        Console.WriteLine($"Deposit: {deposit.Number}");
                        Console.WriteLine($"-Balance: {deposit.Balance:C}");
                        Console.WriteLine($"-Interest Rate: {deposit.InterestRate}%");
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
        static async Task BlockDeposit(AccountControlService accountControlService, Manager manager)
        {
            try
            {
                Console.Write("Deposit number: ");
                var depositNumber = Console.ReadLine();

                await accountControlService.BlockDeposit(manager.Id, depositNumber);
                Console.WriteLine($"\nDeposit '{depositNumber}' blocked");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }
        static async Task UnblockDeposit(AccountControlService accountControlService, Manager manager)
        {
            try
            {
                Console.Write("Deposit number: ");
                var depositNumber = Console.ReadLine();

                await accountControlService.UnblockDeposit(manager.Id, depositNumber);
                Console.WriteLine($"\nDeposit '{depositNumber}' unblocked");
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