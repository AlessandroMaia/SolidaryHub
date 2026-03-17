namespace Campaign.Domain.Events;

public record DonationIntentFailedDomainEvent(
    int DonationIntentId,
    int CampaignId,
    string ErrorMessage) : INotification;
