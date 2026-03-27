using Campaign.API.Application.Commands.DonationIntents.FailDonationIntentProcessing;
using Campaign.API.Application.IntegrationEvents.Events;

namespace Campaign.API.Application.IntegrationEvents.EventHandling;

internal sealed class DonationIntentFailedIntegrationEventHandler(
    ILogger<DonationIntentFailedIntegrationEventHandler> logger,
    IMediator mediator)
    : IIntegrationEventHandler<DonationIntentFailedIntegrationEvent>
{
    public async Task Handle(DonationIntentFailedIntegrationEvent @event)
    {
        logger.LogWarning(
            "Falha no processamento da DonationIntentId {DonationIntentId}: {Reason}",
            @event.DonationIntentId,
            @event.Reason);

        await mediator.Send(new FailDonationIntentProcessingCommand(
            @event.DonationIntentId,
            @event.WorkerName,
            @event.Reason));
    }
}
