namespace Campaign.API.Application.IntegrationEvents;

internal sealed class CampaignIntegrationEventService(
    ILogger<CampaignIntegrationEventService> logger,
    IEventBus eventBus)
    : ICampaignIntegrationEventService, ITransactionEventPublisher
{
    private readonly List<IntegrationEvent> _pendingEvents = [];

    public Task AddAndSaveEventAsync(IntegrationEvent integrationEvent, CancellationToken ct = default)
    {
        _pendingEvents.Add(integrationEvent);

        logger.LogInformation(
            "Evento de integração adicionado ao fluxo de publicação: {IntegrationEventType} ({IntegrationEventId})",
            integrationEvent.GetType().Name,
            integrationEvent.Id);

        return Task.CompletedTask;
    }

    public async Task PublishThroughEventBusAsync(Guid transactionId, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Publicando eventos de integração vinculados à transação {TransactionId}",
            transactionId);

        foreach (var integrationEvent in _pendingEvents)
            await eventBus.PublishAsync(integrationEvent);

        _pendingEvents.Clear();
    }

    public Task PublishAsync(Guid transactionId, CancellationToken cancellationToken = default)
        => PublishThroughEventBusAsync(transactionId, cancellationToken);
}
