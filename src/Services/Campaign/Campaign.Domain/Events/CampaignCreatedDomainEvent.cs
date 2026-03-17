namespace Campaign.Domain.Events;

public record CampaignCreatedDomainEvent(
    int CampaignId,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    decimal FinancialGoalAmount) : INotification;