FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FInanceSystem.Domain/FInanceSystem.Domain.csproj FInanceSystem.Domain/
COPY FinanceSystem.Application/FinanceSystem.Application.csproj FinanceSystem.Application/
COPY FinancialSystem.Infrastructure/FinanceSystem.Infrastructure.csproj FinancialSystem.Infrastructure/
COPY FinanceSystem.API/FinanceSystem.API.csproj FinanceSystem.API/
RUN dotnet restore FinanceSystem.API/FinanceSystem.API.csproj

COPY FInanceSystem.Domain/ FInanceSystem.Domain/
COPY FinanceSystem.Application/ FinanceSystem.Application/
COPY FinancialSystem.Infrastructure/ FinancialSystem.Infrastructure/
COPY FinanceSystem.API/ FinanceSystem.API/
RUN dotnet publish FinanceSystem.API/FinanceSystem.API.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

USER $APP_UID
ENTRYPOINT ["dotnet", "FinanceSystem.API.dll"]
