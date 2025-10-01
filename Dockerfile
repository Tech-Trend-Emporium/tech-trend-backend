# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution & project files first (for restore cache)
COPY TechTrendEmporium.sln ./
COPY TechTrendEmporium/*.csproj TechTrendEmporium/
COPY Application/*.csproj Application/
COPY Domain/*.csproj Domain/
COPY Infrastructure/*.csproj Infrastructure/

# If you actually have NuGet.config at root, you can add:
# COPY NuGet.config ./

# First restore (projects are visible now)
RUN dotnet restore TechTrendEmporium.sln

# Copy the rest of the source
COPY . .

# Second restore (cheap if cached; ensures analyzers/props included)
RUN dotnet restore TechTrendEmporium.sln

# Build & publish API
RUN dotnet build TechTrendEmporium/API.csproj -c $BUILD_CONFIGURATION -o /app/build --no-restore /p:EnableSourceControlManagerQueries=false
RUN dotnet publish TechTrendEmporium/API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:EnableSourceControlManagerQueries=false

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
