using FinanceSystem.Application.Services.AdminServices;
using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.Common;
using FinanceSystem.Application.Services.ManagerServices;
using FinanceSystem.Infrastructure;
using FInanceSystem.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using FinanceSystem.UI.Menus;

namespace FinanceSystem.UI
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddDbContext<FinanceDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

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

                if (dbContext.Database.CanConnect())
                {
                    dbContext.Database.EnsureCreated();
                    await seedService.SeedAllData();
                }
            }

            await MainMenu.Show(serviceProvider);
        }
    }
}