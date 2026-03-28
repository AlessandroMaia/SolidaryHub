using Campaign.API.Application.Commands.DonationIntents.ProcessDonationIntent;

namespace Campaign.API.Application.IntegrationEvents.EventHandling;

internal sealed class DonationIntentProcessedIntegrationEventHandler(
    ILogger<DonationIntentProcessedIntegrationEventHandler> logger,
    IMediator mediator)
    : IIntegrationEventHandler<DonationIntentProcessedIntegrationEvent>
{
    public async Task Handle(DonationIntentProcessedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Processamento concluído para DonationIntentId {DonationIntentId} pelo worker {WorkerName}",
            @event.DonationIntentId,
            @event.WorkerName);

        await mediator.Send(new ProcessDonationIntentCommand(
            @event.DonationIntentId,
            @event.WorkerName));
    }
}
