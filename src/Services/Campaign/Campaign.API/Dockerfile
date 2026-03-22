FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
COPY ["src/Mediator/Mediator.csproj", "src/Mediator/"]
COPY ["src/ServiceDefaults/ServiceDefaults.csproj", "src/ServiceDefaults/"]
COPY ["src/EventBus/EventBus.csproj", "src/EventBus/"]
COPY ["src/Services/Campaign/Campaign.API/Campaign.API.csproj", "src/Services/Campaign/Campaign.API/"]
COPY ["src/Services/Campaign/Campaign.Domain/Campaign.Domain.csproj", "src/Services/Campaign/Campaign.Domain/"]
COPY ["src/Services/Campaign/Campaign.Infrastructure/Campaign.Infrastructure.csproj", "src/Services/Campaign/Campaign.Infrastructure/"]

RUN dotnet restore src/Services/Campaign/Campaign.API/Campaign.API.csproj

COPY . .

RUN dotnet publish src/Services/Campaign/Campaign.API/Campaign.API.csproj \
    -c $BUILD_CONFIGURATION \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

#===============================================================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final

LABEL org.opencontainers.image.title="Campaign API" \
      org.opencontainers.image.description="Campanhas e doações - Solidary Hub" \
      org.opencontainers.image.authors="Alessandro (DEV)" 

WORKDIR /app
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

USER $APP_UID

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Campaign.API.dll"]
