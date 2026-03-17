namespace Campaign.Domain.Events;

public record CampaignCompletedDomainEvent(
    int CampaignId,
    string? Reason) : INotification;