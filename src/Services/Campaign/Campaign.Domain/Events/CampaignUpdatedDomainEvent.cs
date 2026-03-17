namespace Campaign.Domain.Events;

public record CampaignUpdatedDomainEvent(
    int CampaignId,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    decimal FinancialGoalAmount) : INotification;