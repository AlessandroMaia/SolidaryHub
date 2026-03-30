FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]

COPY ["src/BuildingBlocks/SharedKernel/SharedKernel.csproj", "src/BuildingBlocks/SharedKernel/"]
COPY ["src/BuildingBlocks/Mediator/Mediator.csproj", "src/BuildingBlocks/Mediator/"]
COPY ["src/BuildingBlocks/ServiceDefaults/ServiceDefaults.csproj", "src/BuildingBlocks/ServiceDefaults/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "src/BuildingBlocks/EventBusRabbitMQ/"]

COPY ["src/Workers/DonationProcessor/DonationProcessor.csproj", "src/Workers/DonationProcessor/"]

RUN dotnet restore src/Workers/DonationProcessor/DonationProcessor.csproj

COPY . .

RUN dotnet publish src/Workers/DonationProcessor/DonationProcessor.csproj \
    -c $BUILD_CONFIGURATION \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS final

LABEL org.opencontainers.image.title="Donation Processor" \
      org.opencontainers.image.description="Processamento de doações - Solidary Hub" \
      org.opencontainers.image.authors="Alessandro (DEV)" 

WORKDIR /app
EXPOSE 9464
USER $APP_UID

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DonationProcessor.dll"]
