namespace Campaign.API.Application.IntegrationEvents.Events;

public sealed record DonationIntentReceivedIntegrationEvent(
    int CampaignId,
    int DonorUserId,
    decimal Amount,
    string? CorrelationId) : IntegrationEvent;
