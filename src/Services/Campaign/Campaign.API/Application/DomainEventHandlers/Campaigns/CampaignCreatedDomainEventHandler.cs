namespace Campaign.API.Application.DomainEventHandlers.Campaigns;

internal sealed class CampaignCreatedDomainEventHandler(ILogger<CampaignCreatedDomainEventHandler> logger)
    : INotificationHandler<CampaignCreatedDomainEvent>
{
    public Task Handle(CampaignCreatedDomainEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "CampaignCreatedDomainEvent tratado para campanha {CampaignTitle}",
            notification.Title);

        return Task.CompletedTask;
    }
}
