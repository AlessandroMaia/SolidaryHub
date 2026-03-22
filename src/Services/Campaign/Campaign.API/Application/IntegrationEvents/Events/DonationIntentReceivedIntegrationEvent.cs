namespace Campaign.API.Application.IntegrationEvents.Events;

public sealed record DonationIntentReceivedIntegrationEvent(
    int DonationIntentId,
    int CampaignId,
    int DonorUserId,
    decimal Amount,
    string? CorrelationId) : IntegrationEvent;
