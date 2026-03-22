namespace Campaign.API.Application.DomainEventHandlers.DonationIntents;

internal sealed class DonationIntentProcessedDomainEventHandler(ILogger<DonationIntentProcessedDomainEventHandler> logger)
    : INotificationHandler<DonationIntentProcessedDomainEvent>
{
    public Task Handle(DonationIntentProcessedDomainEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "DonationIntentProcessedDomainEvent tratado para intenção {DonationIntentId}",
            notification.DonationIntentId);

        return Task.CompletedTask;
    }
}
