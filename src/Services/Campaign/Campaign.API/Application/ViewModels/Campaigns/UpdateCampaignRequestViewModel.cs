namespace Campaign.API.Application.ViewModels.Campaigns;

public sealed record UpdateCampaignRequestViewModel(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal FinancialGoalAmount);