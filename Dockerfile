# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only csproj files first for better restore caching
COPY TechTrendEmporium/*.csproj TechTrendEmporium/
COPY Application/*.csproj Application/
COPY Domain/*.csproj Domain/
COPY Infrastructure/*.csproj Infrastructure/
# If you have a solution file, copy it too (recommended)
COPY *.sln ./

# Restore
RUN dotnet restore ./TechTrendEmporium/TechTrendEmporium.csproj

# Now copy the rest of the source
COPY . .

# Build
RUN dotnet build ./TechTrendEmporium/TechTrendEmporium.csproj -c $BUILD_CONFIGURATION -o /app/build --no-restore

# Publish
RUN dotnet publish ./TechTrendEmporium/TechTrendEmporium.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
# If your API uses ASPNETCORE_URLS, this matches ACA defaults
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechTrendEmporium.dll"]
