using FinanceSystem.Application.Services.ClientServices;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FinanceSystem.UI.Menus
{
    public static class MainMenu
    {
        public static async Task Show(ServiceProvider serviceProvider)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Finance system\n");

                Console.WriteLine("1. Login as Client");
                Console.WriteLine("2. Login as Manager");
                Console.WriteLine("3. Login as Administrator");
                Console.WriteLine("4. Register new Client");
                Console.WriteLine("0. Exit");

                Console.Write("\nSelect option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ClientMenu.Show(serviceProvider);
                        break;
                    case "2":
                        await ManagerMenu.Show(serviceProvider);
                        break;
                    case "3":
                        await AdminMenu.Show(serviceProvider);
                        break;
                    case "4":
                        await RegisterClient(serviceProvider);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice!");
                        Thread.Sleep(2000);
                        break;
                }
            }
        }

        static async Task RegisterClient(ServiceProvider serviceProvider)
        {
            var registrationService = serviceProvider.GetRequiredService<ClientRegistrationService>();

            Console.Clear();
            Console.WriteLine("Client registration\n");

            Console.Write("Login: ");
            var login = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.Write("First Name: ");
            var firstName = Console.ReadLine();
            Console.Write("Last Name: ");
            var lastName = Console.ReadLine();

            try
            {
                var client = await registrationService.Register(login, password, firstName, lastName);
                Console.WriteLine($"\nClient registered!");
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Thread.Sleep(2000);
            }
        }
    }
}