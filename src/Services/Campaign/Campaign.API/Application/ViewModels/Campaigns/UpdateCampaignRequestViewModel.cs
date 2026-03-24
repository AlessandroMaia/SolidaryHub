namespace Campaign.API.Application.ViewModels.Campaigns;

public sealed record UpdateCampaignRequestViewModel(
    string Title,
    string Description,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    decimal FinancialGoalAmount);
