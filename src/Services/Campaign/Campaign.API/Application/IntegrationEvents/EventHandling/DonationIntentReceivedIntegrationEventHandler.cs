using Campaign.API.Application.Commands.DonationIntents.ProcessDonationIntent;

namespace Campaign.API.Application.IntegrationEvents.EventHandling;

internal sealed class DonationIntentReceivedIntegrationEventHandler(
    ILogger<DonationIntentReceivedIntegrationEventHandler> logger,
    IMediator mediator)
        : IIntegrationEventHandler<DonationIntentReceivedIntegrationEvent>
{
    public async Task Handle(DonationIntentReceivedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Evento de integração de intenção de doação recebido. " +
            "Intenção: {DonationIntentId}, Campanha: {CampaignId}, Doador: {DonorUserId}, Valor: {Amount}, CorrelationId: {CorrelationId}",
            @event.DonationIntentId,
            @event.CampaignId,
            @event.DonorUserId,
            @event.Amount,
            @event.CorrelationId);

        await mediator.Send(new ProcessDonationIntentCommand(
            @event.DonationIntentId,
            "rabbitmq-worker"));
    }
}
