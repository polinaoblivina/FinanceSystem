using FinanceSystem.Infrastructure;
using FInanceSystem.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FinanceDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    try
    {
        Console.WriteLine("Проверка подключения...");
        var canConnect = dbContext.Database.CanConnect();
        Console.WriteLine($"Подключение успешно: {canConnect}");

        if (canConnect)
        {
            Console.WriteLine("Создание базы данных");
            dbContext.Database.EnsureCreated();
            Console.WriteLine("База данных создана");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Ошибка: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
