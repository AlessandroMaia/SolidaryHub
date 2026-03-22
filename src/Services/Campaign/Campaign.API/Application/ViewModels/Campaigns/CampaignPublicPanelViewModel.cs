namespace Campaign.API.Application.ViewModels.Campaigns;

public sealed record CampaignPublicPanelViewModel(
    int Id,
    string Title,
    decimal FinancialGoalAmount,
    decimal TotalAmountRaised);
