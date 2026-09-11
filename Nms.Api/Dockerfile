# ============================================================================
# STAGE 1: Restore & Cache Dependencies
# ============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for optimized layer caching
COPY ["Nms.Api/Nms.Api.csproj", "Nms.Api/"]
COPY ["Nms.Application/Nms.Application.csproj", "Nms.Application/"]
COPY ["Nms.Domain/Nms.Domain.csproj", "Nms.Domain/"]
COPY ["Nms.Infrastructure/Nms.Infrastructure.csproj", "Nms.Infrastructure/"]

# Restore packages
RUN dotnet restore "Nms.Api/Nms.Api.csproj"

# Copy remaining source files
COPY . .

# Build application
WORKDIR "/src/Nms.Api"
RUN dotnet build "Nms.Api.csproj" -c Release --no-restore -o /app/build

# ============================================================================
# STAGE 2: Publish Application
# ============================================================================
FROM build AS publish
RUN dotnet publish "Nms.Api.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

# ============================================================================
# STAGE 3: Lightweight Non-Root Production Runtime
# ============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Install curl for HTTP health check probing
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Non-root container security
USER $APP_UID

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Nms.Api.dll"]