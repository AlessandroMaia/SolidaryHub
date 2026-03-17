namespace Campaign.Domain.Events;

public record CampaignCancelledDomainEvent(
    int CampaignId,
    string? Reason) : INotification;