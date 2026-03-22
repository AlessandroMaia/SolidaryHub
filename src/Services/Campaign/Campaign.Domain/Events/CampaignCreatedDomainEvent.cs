namespace Campaign.Domain.Events;

public record CampaignCreatedDomainEvent(
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    decimal FinancialGoalAmount) : INotification;
