# Этап 1. Сборка
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["InventoryService/InventoryService.csproj", "InventoryService/"]
RUN dotnet restore "InventoryService/InventoryService.csproj"

COPY . .

RUN dotnet publish "InventoryService/InventoryService.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Этап 2. Запуск
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

USER app

EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "InventoryService.dll"]