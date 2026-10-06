using FinanceSystem.API.Infrastructure;
using FinanceSystem.API.Services;
using FinanceSystem.Application.Services.AdminServices;
using FinanceSystem.Application.Services.ClientServices;
using FinanceSystem.Application.Services.Common;
using FinanceSystem.Application.Services.ManagerServices;
using FinanceSystem.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

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

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<JwtTokenService>();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be configured and contain at least 32 characters.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceSystem API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT token from /api/Auth/login"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();

    await dbContext.Database.EnsureCreatedAsync();
    await seedService.SeedAllData();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
