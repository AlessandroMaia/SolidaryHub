namespace Campaign.API.Application.Commands.DonationIntents.CreateDonationIntent;

public sealed record CreateDonationIntentCommand(
    int CampaignId,
    int DonorUserId,
    decimal Amount,
    string Currency,
    string Source,
    string? CorrelationId,
    string? MessageId,
    Guid RequestId = default) : IIdempotentCommand<int>;
