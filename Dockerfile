# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Domain/GymAPI.Domain.csproj src/Domain/
COPY src/Application/GymAPI.Application.csproj src/Application/
COPY src/Infrastructure/GymAPI.Infrastructure.csproj src/Infrastructure/
COPY src/Api/GymAPI.Api.csproj src/Api/

RUN dotnet restore src/Api/GymAPI.Api.csproj --force --ignore-failed-sources

COPY src/Domain/ src/Domain/
COPY src/Application/ src/Application/
COPY src/Infrastructure/ src/Infrastructure/
COPY src/Api/ src/Api/

RUN dotnet publish src/Api/GymAPI.Api.csproj -c Release -o /app/publish

# ============================================
# Stage 2: Runtime (Debian slim)
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    libgssapi-krb5-2 && \
    rm -rf /var/lib/apt/lists/*

RUN groupadd -r gymapi && useradd -r -g gymapi -d /app -s /sbin/nologin gymapi

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/logs && chown -R gymapi:gymapi /app

USER gymapi

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "GymAPI.Api.dll"]
