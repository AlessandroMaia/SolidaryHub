namespace Campaign.API.Application.IntegrationEvents.Events;

public sealed record DonationIntentFailedIntegrationEvent(
    int DonationIntentId,
    string WorkerName,
    string Reason) : IntegrationEvent;
