namespace Campaign.API.Application.DomainEventHandlers.DonationIntents;

internal sealed class DonationIntentCreatedDomainEventHandler(ILogger<DonationIntentCreatedDomainEventHandler> logger)
    : INotificationHandler<DonationIntentCreatedDomainEvent>
{
    public Task Handle(DonationIntentCreatedDomainEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "DonationIntentCreatedDomainEvent tratado para intenção {DonationIntentId}",
            notification.DonationIntentId);

        return Task.CompletedTask;
    }
}
