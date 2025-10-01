# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution & project files first for better cache
COPY TechTrendEmporium.sln ./
COPY TechTrendEmporium/*.csproj TechTrendEmporium/
COPY Application/*.csproj Application/
COPY Domain/*.csproj Domain/
COPY Infrastructure/*.csproj Infrastructure/

# Restore using the solution (pulls all referenced projects)
RUN dotnet restore TechTrendEmporium.sln

# Now copy the rest of the source
COPY . .

# Build & publish the API project explicitly
RUN dotnet build TechTrendEmporium/API.csproj -c $BUILD_CONFIGURATION -o /app/build --no-restore
RUN dotnet publish TechTrendEmporium/API.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-build

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
