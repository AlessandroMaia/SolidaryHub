namespace Campaign.API.Application.IntegrationEvents;

public interface ICampaignIntegrationEventService
{
    Task AddAndSaveEventAsync(
        IntegrationEvent integrationEvent,
        CancellationToken ct = default);

    Task PublishThroughEventBusAsync(
        Guid transactionId,
        CancellationToken ct = default);
}
