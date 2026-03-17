namespace Campaign.Domain.Events;

public record CampaignTotalUpdatedDomainEvent(
    int CampaignId,
    decimal TotalAmountRaised,
    int TotalDonationsCount) : INotification;