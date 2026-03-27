namespace Campaign.API.Application.IntegrationEvents.Events;

public sealed record DonationIntentProcessedIntegrationEvent(
    int DonationIntentId,
    string WorkerName) : IntegrationEvent;
