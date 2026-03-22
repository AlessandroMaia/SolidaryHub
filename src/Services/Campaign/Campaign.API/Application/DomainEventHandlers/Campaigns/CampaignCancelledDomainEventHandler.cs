namespace Campaign.API.Application.DomainEventHandlers.Campaigns;

internal sealed class CampaignCancelledDomainEventHandler(ILogger<CampaignCancelledDomainEventHandler> logger)
    : INotificationHandler<CampaignCancelledDomainEvent>
{
    public Task Handle(CampaignCancelledDomainEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "CampaignCancelledDomainEvent tratado para campanha {CampaignId}",
            notification.CampaignId);

        return Task.CompletedTask;
    }
}
