# syntax=docker/dockerfile:1.6

# --- Build stage ---
ARG DOTNET_VERSION=8.0
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build

# Build-time args you can override from the workflow
ARG BUILD_CONFIGURATION=Release
# Path to the API .csproj (relative to repo root)
ARG PROJECT_PATH=API/TechTrendEmporium.API.csproj

WORKDIR /src

# Copy solution and project files first to maximize layer caching
# Adjust these COPY lines if your folder names differ.
COPY TechTrendEmporium.sln ./
COPY API/*.csproj ./API/
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infrastructure/*.csproj ./Infrastructure/

# Restore dependencies
RUN dotnet restore ${PROJECT_PATH}

# Now copy the entire repo and publish
COPY . .
RUN dotnet publish ${PROJECT_PATH} -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# Container-friendly default for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Name of the built DLL to run (override if different)
ARG PROJECT_DLL=TechTrendEmporium.API.dll

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "${PROJECT_DLL}"]
