using System.Diagnostics;
using DonationProcessor.Observability;

namespace DonationProcessor.IntegrationEvents.EventHandling;

public class DonationIntentProcessingIntegrationEventHandler(
    IEventBus eventBus,
    IOptionsMonitor<DonationOptions> options,
    ILogger<DonationIntentProcessingIntegrationEventHandler> logger)
        : IIntegrationEventHandler<DonationIntentProcessingIntegrationEvent>
{
    public async Task Handle(DonationIntentProcessingIntegrationEvent @event)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("Iniciando evento de integração: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
        DonationProcessorMetrics.MessageConsumed(@event.WorkerName);

        IntegrationEvent integrationEvent;

        if (options.CurrentValue.Succeded)
        {
            integrationEvent = new DonationIntentProcessedIntegrationEvent(@event.DonationIntentId, @event.WorkerName);
            DonationProcessorMetrics.MessageProcessed(@event.WorkerName, stopwatch);
        }
        else
        {
            integrationEvent = new DonationIntentFailedIntegrationEvent(@event.DonationIntentId, @event.WorkerName, "falhou");
            DonationProcessorMetrics.MessageFailed(@event.WorkerName, stopwatch);
        }

        logger.LogInformation("Publicando evento de integração: {IntegrationEventId} - ({@IntegrationEvent})", integrationEvent.Id, integrationEvent);

        await eventBus.PublishAsync(integrationEvent);
    }
}
