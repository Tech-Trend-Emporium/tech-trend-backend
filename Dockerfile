# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
# Adjustable via docker-compose
ARG PROJECT_PATH=TechTrendEmporium/API.csproj
ARG PROJECT_DLL=API.dll

WORKDIR /src

# Copy solution and project folders
COPY TechTrendEmporium.sln ./
COPY TechTrendEmporium/ TechTrendEmporium/
COPY Application/ Application/
COPY Domain/ Domain/
COPY Infrastructure/ Infrastructure/
COPY Starter/ Starter/

# Restore dependencies for the entire solution (more robust)
RUN dotnet restore "TechTrendEmporium.sln"

# Build the web project
RUN dotnet build "$PROJECT_PATH" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG PROJECT_PATH=TechTrendEmporium/API.csproj
RUN dotnet publish "$PROJECT_PATH" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Start the correct DLL
ARG PROJECT_DLL=API.dll
ENTRYPOINT ["dotnet", "API.dll"]