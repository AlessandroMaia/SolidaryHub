namespace Campaign.API.Application.IntegrationEvents.Events;

public sealed record DonationIntentProcessingIntegrationEvent(
    int DonationIntentId,
    string WorkerName) : IntegrationEvent;
