namespace Campaign.API.Application.ViewModels.DonationIntents;

public sealed record DonationIntentDetailsViewModel(
    int Id,
    int CampaignId,
    int DonorUserId,
    decimal Amount,
    string Currency,
    string Status,
    string Source,
    string? CorrelationId,
    string? MessageId,
    DateTime RequestedAt,
    DateTime? ValidatedAt,
    DateTime? RejectedAt,
    DateTime? ProcessedAt,
    string? RejectionReason);