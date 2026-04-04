FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]

COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "src/BuildingBlocks/EventBusRabbitMQ/"]
COPY ["src/BuildingBlocks/Mediator/Mediator.csproj", "src/BuildingBlocks/Mediator/"]
COPY ["src/BuildingBlocks/ServiceDefaults/ServiceDefaults.csproj", "src/BuildingBlocks/ServiceDefaults/"]
COPY ["src/BuildingBlocks/SharedKernel/SharedKernel.csproj", "src/BuildingBlocks/SharedKernel/"]
COPY ["src/Gateways/ApiGateway/ApiGateway.csproj", "src/Gateways/ApiGateway/"]

RUN dotnet restore src/Gateways/ApiGateway/ApiGateway.csproj

COPY . .

RUN dotnet publish src/Gateways/ApiGateway/ApiGateway.csproj \
    -c $BUILD_CONFIGURATION \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final

LABEL org.opencontainers.image.title="Api Gateway" \
      org.opencontainers.image.description="YARP API Gateway - Solidary Hub" \
      org.opencontainers.image.authors="Alessandro (DEV)"

WORKDIR /app
EXPOSE 8080
USER $APP_UID

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
