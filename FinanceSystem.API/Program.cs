using FinanceSystem.API.Services;
using FinanceSystem.Application.Services.AdminServices;
using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.Common;
using FinanceSystem.Application.Services.ManagerServices;
using FinanceSystem.Infrastructure;
using FInanceSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FinanceDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Admin
builder.Services.AddScoped<LogViewerService>();
builder.Services.AddScoped<ActionRevertService>();

//Manager
builder.Services.AddScoped<ClientManagementService>();
builder.Services.AddScoped<EnterpriseManagementService>();
builder.Services.AddScoped<SalaryRequestService>();
builder.Services.AddScoped<AccountControlService>();

//Client
builder.Services.AddScoped<ClientRegistrationService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<DepositService>();
builder.Services.AddScoped<TransferService>();
builder.Services.AddScoped<InfoService>();
builder.Services.AddScoped<SalaryService>();

//Common
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SeedService>();

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceSystem API", Version = "v1" });

    c.AddSecurityDefinition("X-UserId", new OpenApiSecurityScheme
    {
        Name = "X-UserId",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Enter Id"
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();

    try
    {
        if (dbContext.Database.CanConnect())
        {
            dbContext.Database.EnsureCreated();
            await seedService.SeedAllData();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Ошибка: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
