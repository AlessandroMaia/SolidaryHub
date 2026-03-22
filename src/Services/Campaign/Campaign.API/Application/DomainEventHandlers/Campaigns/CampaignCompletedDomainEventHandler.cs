namespace Campaign.API.Application.DomainEventHandlers.Campaigns;

internal sealed class CampaignCompletedDomainEventHandler(ILogger<CampaignCompletedDomainEventHandler> logger)
    : INotificationHandler<CampaignCompletedDomainEvent>
{
    public Task Handle(CampaignCompletedDomainEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "CampaignCompletedDomainEvent tratado para campanha {CampaignId}",
            notification.CampaignId);

        return Task.CompletedTask;
    }
}
