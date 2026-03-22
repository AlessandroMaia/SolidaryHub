namespace Campaign.API.Application.DomainEventHandlers.Campaigns;

internal sealed class CampaignUpdatedDomainEventHandler(ILogger<CampaignUpdatedDomainEventHandler> logger)
    : INotificationHandler<CampaignUpdatedDomainEvent>
{
    public Task Handle(CampaignUpdatedDomainEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "CampaignUpdatedDomainEvent tratado para campanha {CampaignId}",
            notification.CampaignId);

        return Task.CompletedTask;
    }
}
