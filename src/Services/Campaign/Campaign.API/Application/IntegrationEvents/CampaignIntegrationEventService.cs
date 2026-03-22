namespace Campaign.API.Application.IntegrationEvents;

internal sealed class CampaignIntegrationEventService(
    ILogger<CampaignIntegrationEventService> logger)
    : ICampaignIntegrationEventService
{
    public Task AddAndSaveEventAsync(
        IntegrationEvent integrationEvent,
        CancellationToken ct = default)
    {
        logger.LogInformation(
            "Integration event adicionado ao fluxo de publicação: {IntegrationEventType} ({IntegrationEventId})",
            integrationEvent.GetType().Name,
            integrationEvent.Id);

        return Task.CompletedTask;
    }

    public Task PublishThroughEventBusAsync(
        Guid transactionId,
        CancellationToken ct = default)
    {
        logger.LogInformation(
            "Publicando integration events vinculados à transação {TransactionId}",
            transactionId);

        return Task.CompletedTask;
    }
}
