using FinanceSystem.Application.Facades;
using FinanceSystem.Infrastructure;
using FInanceSystem.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FinanceDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    try
    {
        var validConnection = dbContext.Database.CanConnect();

        if (validConnection)
            dbContext.Database.EnsureCreated();
        

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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
