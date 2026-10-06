using FinanceSystem.Application.Services.AdminServices;
using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.Common;
using FinanceSystem.Application.Services.ManagerServices;
using FinanceSystem.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinanceSystem.UI.Menus;

namespace FinanceSystem.UI
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();

            services.AddInfrastructure(configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

            services.AddScoped<LogViewerService>();
            services.AddScoped<ActionRevertService>();

            services.AddScoped<ClientManagementService>();
            services.AddScoped<EnterpriseManagementService>();
            services.AddScoped<SalaryRequestService>();
            services.AddScoped<AccountControlService>();

            services.AddScoped<ClientRegistrationService>();
            services.AddScoped<AccountService>();
            services.AddScoped<DepositService>();
            services.AddScoped<TransferService>();
            services.AddScoped<InfoService>();
            services.AddScoped<SalaryService>();

            services.AddScoped<AuthService>();
            services.AddScoped<SeedService>();

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
                var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();

                await dbContext.Database.EnsureCreatedAsync();
                await seedService.SeedAllData();
            }

            await MainMenu.Show(serviceProvider);
        }
    }
}