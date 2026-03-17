namespace Campaign.Domain.Events;

public record DonationIntentRejectedDomainEvent(
    int DonationIntentId,
    int CampaignId,
    string Reason) : INotification;
