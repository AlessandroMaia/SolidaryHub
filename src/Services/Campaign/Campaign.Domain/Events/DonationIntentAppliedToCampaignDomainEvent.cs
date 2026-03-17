namespace Campaign.Domain.Events;

public record DonationIntentAppliedToCampaignDomainEvent(
    int CampaignId,
    Guid DonationIntentId,
    decimal Amount) : INotification;
