# syntax=docker/dockerfile:1.6

ARG DOTNET_VERSION=8.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
SHELL ["/bin/bash","-c"]

# Build-time knobs (override via build-args if needed)
ARG BUILD_CONFIGURATION=Release
# Match your API project file (any depth). Adjust if your API project doesn't end in .API.csproj
ARG API_PROJECT_GLOB="*API.csproj"

WORKDIR /src

# Because the build context is the TechTrendEmporium folder, copy everything from there.
COPY . .

# Restore via the solution if present (helps cache); otherwise we’ll restore during publish anyway.
RUN if [[ -f TechTrendEmporium.sln ]]; then dotnet restore TechTrendEmporium.sln; else echo "No solution file found (ok)"; fi

# Discover the API project and publish it
RUN set -euo pipefail; \
    PROJECT_PATH=$(find . -type f -name "${API_PROJECT_GLOB}" | head -n1); \
    if [[ -z "$PROJECT_PATH" ]]; then echo "Could not find API project matching ${API_PROJECT_GLOB}" >&2; exit 1; fi; \
    echo "Using API project: $PROJECT_PATH"; \
    dotnet publish "$PROJECT_PATH" -c "${BUILD_CONFIGURATION}" -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# Default ASP.NET Core port
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Find the API DLL automatically at runtime
ARG API_DLL_GLOB="*.API.dll"

COPY --from=build /app/publish .

# Start the first *.API.dll we find
ENTRYPOINT ["/bin/bash","-lc","exec dotnet \"$(ls ${API_DLL_GLOB} | head -n1)\""]
