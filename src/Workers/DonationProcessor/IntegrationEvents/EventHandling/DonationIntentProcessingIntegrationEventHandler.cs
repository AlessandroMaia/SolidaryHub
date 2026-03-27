namespace DonationProcessor.IntegrationEvents.EventHandling;

public class DonationIntentProcessingIntegrationEventHandler(
    IEventBus eventBus,
    IOptionsMonitor<DonationOptions> options,
    ILogger<DonationIntentProcessingIntegrationEventHandler> logger)
        : IIntegrationEventHandler<DonationIntentProcessingIntegrationEvent>
{
    public async Task Handle(DonationIntentProcessingIntegrationEvent @event)
    {
        logger.LogInformation("Iniciando evento de integração: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        IntegrationEvent integrationEvent;

        if (options.CurrentValue.Succeeded)
            integrationEvent = new DonationIntentProcessedIntegrationEvent(@event.DonationIntentId, @event.WorkerName);
        else
            integrationEvent = new DonationIntentFailedIntegrationEvent(@event.DonationIntentId, @event.WorkerName, "falhou");

        logger.LogInformation("Publicando evento de integração: {IntegrationEventId} - ({@IntegrationEvent})", integrationEvent.Id, integrationEvent);

        await eventBus.PublishAsync(integrationEvent);
    }
}
