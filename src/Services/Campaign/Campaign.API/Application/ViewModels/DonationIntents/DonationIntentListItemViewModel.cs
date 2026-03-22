namespace Campaign.API.Application.ViewModels.DonationIntents;

public sealed record DonationIntentListItemViewModel(
    int Id,
    int CampaignId,
    int DonorUserId,
    decimal Amount,
    string Currency,
    string Status,
    DateTime RequestedAt,
    DateTime? ProcessedAt);
