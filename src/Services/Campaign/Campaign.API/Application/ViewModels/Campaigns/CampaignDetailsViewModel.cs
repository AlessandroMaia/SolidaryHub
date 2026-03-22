namespace Campaign.API.Application.ViewModels.Campaigns;

public sealed record CampaignDetailsViewModel(
    int Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal FinancialGoalAmount,
    decimal TotalAmountRaised,
    int TotalDonationsCount,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
