namespace Campaign.API.Application.DomainEventHandlers.DonationIntents;

internal sealed class DonationIntentFailedDomainEventHandler(ILogger<DonationIntentFailedDomainEventHandler> logger)
    : INotificationHandler<DonationIntentFailedDomainEvent>
{
    public Task Handle(DonationIntentFailedDomainEvent notification, CancellationToken ct)
    {
        logger.LogWarning(
            "DonationIntentFailedDomainEvent tratado para intenção {DonationIntentId}",
            notification.DonationIntentId);

        return Task.CompletedTask;
    }
}
