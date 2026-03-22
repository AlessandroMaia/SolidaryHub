namespace Campaign.Domain.Events;

public record DonationIntentCreatedDomainEvent(
    int CampaignId,
    int DonorUserId,
    decimal Amount,
    string? CorrelationId) : INotification;