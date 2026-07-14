# === STAGE 1: BUILD ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Pacotes locais (Fgc.MessageContracts)
COPY LocalPackages ./LocalPackages
COPY nuget.config .
# Restore em cache (apenas os .csproj)
COPY ["Fgc.Catalog/src/Fgc.Catalog.Api/Fgc.Catalog.Api.csproj", "Fgc.Catalog/src/Fgc.Catalog.Api/"]
COPY ["Fgc.Catalog/src/Fgc.Catalog.Application/Fgc.Catalog.Application.csproj", "Fgc.Catalog/src/Fgc.Catalog.Application/"]
COPY ["Fgc.Catalog/src/Fgc.Catalog.Domain/Fgc.Catalog.Domain.csproj", "Fgc.Catalog/src/Fgc.Catalog.Domain/"]
COPY ["Fgc.Catalog/src/Fgc.Catalog.Infrastructure/Fgc.Catalog.Infrastructure.csproj", "Fgc.Catalog/src/Fgc.Catalog.Infrastructure/"]
RUN dotnet restore "Fgc.Catalog/src/Fgc.Catalog.Api/Fgc.Catalog.Api.csproj"
# Build e publish
COPY . .
WORKDIR "/src/Fgc.Catalog/src/Fgc.Catalog.Api"
RUN dotnet publish "Fgc.Catalog.Api.csproj" -c Release -o /app/publish
# === STAGE 2: RUNTIME ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fgc.Catalog.Api.dll"]