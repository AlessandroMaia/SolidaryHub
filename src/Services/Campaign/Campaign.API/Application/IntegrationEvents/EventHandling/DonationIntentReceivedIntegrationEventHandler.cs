namespace Campaign.API.Application.IntegrationEvents.EventHandling;

internal sealed class DonationIntentReceivedIntegrationEventHandler(
    ILogger<DonationIntentReceivedIntegrationEventHandler> logger)
        : IIntegrationEventHandler<DonationIntentReceivedIntegrationEvent>
{
    public Task Handle(DonationIntentReceivedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Evento de integração de intenção de doação recebido. " +
            "Campanha: {CampaignId}, Doador: {DonorUserId}, Valor: {Amount}, CorrelationId: {CorrelationId}",
            @event.CampaignId,
            @event.DonorUserId,
            @event.Amount,
            @event.CorrelationId);

        return Task.CompletedTask;
    }
}
