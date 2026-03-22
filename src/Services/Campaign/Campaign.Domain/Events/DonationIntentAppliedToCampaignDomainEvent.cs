namespace Campaign.Domain.Events;

public record DonationIntentAppliedToCampaignDomainEvent(
    int CampaignId,
    int DonationIntentId,
    decimal Amount) : INotification;
