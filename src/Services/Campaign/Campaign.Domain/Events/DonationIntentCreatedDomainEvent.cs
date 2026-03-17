namespace Campaign.Domain.Events;

public record DonationIntentCreatedDomainEvent(
    int DonationIntentId,
    int CampaignId,
    int DonorUserId,
    decimal Amount) : INotification;
