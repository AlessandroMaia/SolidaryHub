using Campaign.API.Application.Commands.DonationIntents.StartDonationIntentProcessing;
using Campaign.API.Application.IntegrationEvents.Events;

namespace Campaign.API.Application.IntegrationEvents.EventHandling;

internal sealed class DonationIntentProcessingStartedIntegrationEventHandler(
    ILogger<DonationIntentProcessingStartedIntegrationEventHandler> logger,
    IMediator mediator)
    : IIntegrationEventHandler<DonationIntentProcessingStartedIntegrationEvent>
{
    public async Task Handle(DonationIntentProcessingStartedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Processamento iniciado para DonationIntentId {DonationIntentId} pelo worker {WorkerName}",
            @event.DonationIntentId,
            @event.WorkerName);

        await mediator.Send(new StartDonationIntentProcessingCommand(
            @event.DonationIntentId,
            @event.WorkerName));
    }
}
