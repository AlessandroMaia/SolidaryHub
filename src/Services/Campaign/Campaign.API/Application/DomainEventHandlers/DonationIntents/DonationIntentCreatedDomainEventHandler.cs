namespace Campaign.API.Application.DomainEventHandlers.DonationIntents;

internal sealed class DonationIntentCreatedDomainEventHandler(
    ILogger<DonationIntentCreatedDomainEventHandler> logger,
    ICampaignIntegrationEventService integrationEventService)
        : INotificationHandler<DonationIntentCreatedDomainEvent>
{
    public async Task Handle(DonationIntentCreatedDomainEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "DonationIntentCreatedDomainEvent tratado para campanha {CampaignId}",
            notification.CampaignId);

        await integrationEventService.AddAndSaveEventAsync(
            new DonationIntentReceivedIntegrationEvent(
                notification.CampaignId,
                notification.DonorUserId,
                notification.Amount,
                notification.CorrelationId),
            ct);
    }
}
