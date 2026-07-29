# ============================================================================
# STAGE 1: Build & Publish Application
# ============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for optimized layer caching
COPY ["Nms.Api/Nms.Api.csproj", "Nms.Api/"]
COPY ["Nms.Application/Nms.Application.csproj", "Nms.Application/"]
COPY ["Nms.Domain/Nms.Domain.csproj", "Nms.Domain/"]
COPY ["Nms.Infrastructure/Nms.Infrastructure.csproj", "Nms.Infrastructure/"]

# Restore packages
RUN dotnet restore "Nms.Api/Nms.Api.csproj"

# Copy full source and build
COPY . .
WORKDIR "/src/Nms.Api"
RUN dotnet build "Nms.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nms.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================================================
# STAGE 2: Lightweight Production Runtime
# ============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nms.Api.dll"]