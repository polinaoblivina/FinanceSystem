using FinanceSystem.Application.Services.AdminServices;
using FinanceSystem.Application.Services.Common;
using FInanceSystem.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace FinanceSystem.UI.Menus
{
    public class AdminMenu
    {
        public static async Task Show(ServiceProvider serviceProvider, Admin? admin = null)
        {
            var logViewerService = serviceProvider.GetRequiredService<LogViewerService>();
            var actionRevertService = serviceProvider.GetRequiredService<ActionRevertService>();

            if (admin == null)
            {
                var authService = serviceProvider.GetRequiredService<AuthService>();

                Console.Write("Login: ");
                var login = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();

                try
                {
                    var user = await authService.Login(login, password);
                    admin = user as Admin;
                    if (admin == null)
                    {
                        Console.WriteLine("\nThis user is not an administrator!");
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
                Console.WriteLine($"Administrator: {admin.Login}\n");

                Console.WriteLine("1. Show all logs");
                Console.WriteLine("2. Revert action by log number");
                Console.WriteLine("0. Exit");

                Console.Write("\nSelect option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowAllLogs(logViewerService);
                        break;
                    case "2":
                        await RevertAction(actionRevertService, admin);
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

        static async Task ShowAllLogs(LogViewerService logViewerService)
        {
            Console.WriteLine("\nLogs");

            var logs = await logViewerService.GetAllLogs();

            if (logs.Count == 0)
            {
                Console.WriteLine("Logs not found.");
            }
            else
            {
                foreach (var log in logs)
                {
                    Console.WriteLine($"[{log.Timestamp:yyyy-MM-dd HH:mm:ss}] {log.Action}");
                    Console.WriteLine($"-User ID: {log.UserId}");
                    Console.WriteLine($"-Target: {log.TargetId} ({log.TargetType})");
                    Console.WriteLine($"-Log mumber: {log.Number}");
                    if (!string.IsNullOrEmpty(log.FromInfo))
                        Console.WriteLine($"-From: {log.FromInfo}");
                    if (!string.IsNullOrEmpty(log.ToInfo))
                        Console.WriteLine($"-To: {log.ToInfo}");
                    if (log.Amount.HasValue)
                        Console.WriteLine($"-Amount: {log.Amount.Value:C}");
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        static async Task RevertAction(ActionRevertService actionRevertService, Admin admin)
        {
            Console.Write("Log number: ");
            var logNumber = Console.ReadLine();

            try
            {
                await actionRevertService.RevertByLogId(admin.Id, logNumber);
                Console.WriteLine($"\nAction reverted successfully!");
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