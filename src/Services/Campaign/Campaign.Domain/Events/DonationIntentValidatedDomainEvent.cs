namespace Campaign.Domain.Events;

public record DonationIntentValidatedDomainEvent(
    int DonationIntentId,
    int CampaignId,
    decimal Amount) : INotification;
