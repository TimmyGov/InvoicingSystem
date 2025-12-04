# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/InvoiceSystem.API/InvoiceSystem.API.csproj", "src/InvoiceSystem.API/"]
COPY ["src/InvoiceSystem.Application/InvoiceSystem.Application.csproj", "src/InvoiceSystem.Application/"]
COPY ["src/InvoiceSystem.Infrastructure/InvoiceSystem.Infrastructure.csproj", "src/InvoiceSystem.Infrastructure/"]
COPY ["src/InvoiceSystem.Domain/InvoiceSystem.Domain.csproj", "src/InvoiceSystem.Domain/"]

# Restore dependencies
RUN dotnet restore "src/InvoiceSystem.API/InvoiceSystem.API.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/src/InvoiceSystem.API"
RUN dotnet build "InvoiceSystem.API.csproj" -c Release -o /app/build
RUN dotnet publish "InvoiceSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InvoiceSystem.API.dll"]
