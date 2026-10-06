# FinanceSystem

Учебная банковская система на ASP.NET Core. Клиенты открывают счета и вклады, делают переводы и получают зарплату. Менеджеры одобряют клиентов и заявки и блокируют счета. Администратор просматривает журнал действий и откатывает операции.

Работать с системой можно через REST API или консольное приложение.

## Стек

.NET 9, ASP.NET Core Web API, Entity Framework Core, MySQL, JWT, BCrypt, Swagger, Docker.

## Структура

- FInanceSystem.Domain: сущности и бизнес-правила
- FinanceSystem.Application: сервисы сценариев
- FinancialSystem.Infrastructure: доступ к данным
- FinanceSystem.API: REST API
- FinanceSystem.UI: консольное приложение

## Запуск через Docker

```
cp .env.example .env
docker compose up -d --build
```

В файле .env нужно задать свои пароли и ключ JWT длиной не меньше 32 символов.

Swagger: http://localhost:8080/swagger

## Локальный запуск

Нужны .NET 9 SDK и MySQL. Строку подключения и ключ JWT нужно указать в FinanceSystem.API/appsettings.Development.json:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=FinanceApp;User=root;Password=your-password;"
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long"
  }
}
```

```
dotnet run --project FinanceSystem.API
```

Swagger: http://localhost:5022/swagger

## Учётные записи

- admin / adminparol
- manager / managerparol

Пока менеджер не одобрит клиента, тот не может открывать счета и выполнять другие операции.