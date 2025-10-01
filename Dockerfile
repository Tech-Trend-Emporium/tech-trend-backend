# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1) Copy solution + project files AND central NuGet/props files first
COPY TechTrendEmporium.sln ./
COPY TechTrendEmporium/*.csproj TechTrendEmporium/
COPY Application/*.csproj Application/
COPY Domain/*.csproj Domain/
COPY Infrastructure/*.csproj Infrastructure/

# Central package & config files (copy only if they exist in your repo)
# Add/remove lines as needed for your repo.
COPY NuGet.config* ./
COPY Directory.Packages.props ./
COPY Directory.Build.props ./
COPY Directory.Build.targets ./

# 2) Restore with all metadata present
RUN dotnet restore TechTrendEmporium.sln

# 3) Now copy the rest of the source
COPY . .

# (Optional but robust) Run restore again after full copy.
# This is quick if cache is warm and guarantees analyzers/etc. are present.
RUN dotnet restore TechTrendEmporium.sln

# 4) Build & publish the API project explicitly
RUN dotnet build TechTrendEmporium/API.csproj -c $BUILD_CONFIGURATION -o /app/build --no-restore
RUN dotnet publish TechTrendEmporium/API.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-build

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
