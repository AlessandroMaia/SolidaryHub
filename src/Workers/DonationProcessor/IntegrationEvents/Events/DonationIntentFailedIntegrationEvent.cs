namespace DonationProcessor.IntegrationEvents.Events;

public sealed record DonationIntentFailedIntegrationEvent(
    int DonationIntentId,
    string WorkerName,
    string Reason) : IntegrationEvent;
