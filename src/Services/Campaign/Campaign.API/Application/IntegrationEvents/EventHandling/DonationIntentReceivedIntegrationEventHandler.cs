namespace Campaign.API.Application.IntegrationEvents.EventHandling;

internal sealed class DonationIntentReceivedIntegrationEventHandler(
    ILogger<DonationIntentReceivedIntegrationEventHandler> logger)
        : IIntegrationEventHandler<DonationIntentReceivedIntegrationEvent>
{
    public Task Handle(DonationIntentReceivedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Evento de integração de intenção de doação recebido. " +
            "Intenção: {DonationIntentId}, Campanha: {CampaignId}, Doador: {DonorUserId}, Valor: {Amount}",
            @event.DonationIntentId,
            @event.CampaignId,
            @event.DonorUserId,
            @event.Amount);

        return Task.CompletedTask;
    }
}