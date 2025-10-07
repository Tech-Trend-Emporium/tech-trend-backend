# Runtime (aspnet) — SIN usuario raro por defecto
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Asegura binding en contenedor (escuchar en 0.0.0.0)
ENV ASPNETCORE_URLS=http://+:8080

# Build (sdk)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia solución y TODOS los csproj de proyectos referenciados (lo tienes bien)
COPY ["TechTrendEmporium.sln", "./"]
COPY ["TechTrendEmporium/TechTrendEmporium.csproj", "TechTrendEmporium/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Starter/Starter.csproj", "Starter/"]

RUN dotnet restore "./TechTrendEmporium/TechTrendEmporium.csproj"

# Copia el resto del código
COPY . .

WORKDIR "/src/TechTrendEmporium"
RUN dotnet build "./TechTrendEmporium.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechTrendEmporium.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechTrendEmporium.dll"]
