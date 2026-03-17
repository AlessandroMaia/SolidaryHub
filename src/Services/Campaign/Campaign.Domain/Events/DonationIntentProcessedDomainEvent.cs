namespace Campaign.Domain.Events;

public record DonationIntentProcessedDomainEvent(
    int DonationIntentId,
    int CampaignId,
    int DonorUserId,
    decimal Amount,
    DateTime ProcessedAt) : INotification;
