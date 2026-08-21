# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["InvoicesService.Api/InvoicesService.Api.csproj", "InvoicesService.Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["InvoicesService.Shared.Contracts/InvoicesService.Shared.Contracts.csproj", "InvoicesService.Shared.Contracts/"]

RUN dotnet restore "InvoicesService.Api/InvoicesService.Api.csproj"

COPY . .

RUN dotnet publish "InvoicesService.Api/InvoicesService.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "InvoicesService.Api.dll"]