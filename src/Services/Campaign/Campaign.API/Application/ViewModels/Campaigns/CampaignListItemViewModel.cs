namespace Campaign.API.Application.ViewModels.Campaigns;

public sealed record CampaignListItemViewModel(
    int Id,
    string Title,
    decimal FinancialGoalAmount,
    decimal TotalAmountRaised,
    string Status,
    DateTime StartDate,
    DateTime EndDate);