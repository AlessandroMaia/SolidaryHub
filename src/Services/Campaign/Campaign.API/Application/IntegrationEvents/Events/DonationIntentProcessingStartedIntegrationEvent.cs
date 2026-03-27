namespace Campaign.API.Application.IntegrationEvents.Events;

public sealed record DonationIntentProcessingStartedIntegrationEvent(
    int DonationIntentId,
    string WorkerName) : IntegrationEvent;
