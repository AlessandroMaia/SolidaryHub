namespace Campaign.API.Application.Commands.DonationIntents.CreateDonationIntent;

internal sealed class CreateDonationIntentCommandHandler(
    IDonationIntentRepository donationIntentRepository,
    ICampaignRepository campaignRepository,
    ICampaignIntegrationEventService integrationEventService)
        : ICommandHandler<CreateDonationIntentCommand, int>
{
    public async Task<int> Handle(CreateDonationIntentCommand command, CancellationToken ct)
    {
        var campaign = await campaignRepository.GetByIdAsync(command.CampaignId, ct)
            ?? throw new CampaignDomainException("Campanha não encontrada");

        if (campaign.Status != CampaignStatus.Active)
            throw new CampaignDomainException("A campanha não está ativa para receber doações");

        if (campaign.Period.IsExpired(DateTime.UtcNow))
            throw new CampaignDomainException("A campanha está encerrada");

        var donationIntent = DonationIntent.Create(
            command.CampaignId,
            command.DonorUserId,
            command.Amount,
            command.Currency,
            command.Source,
            command.CorrelationId,
            command.MessageId);

        donationIntentRepository.Add(donationIntent);
        await donationIntentRepository.UnitOfWork.SaveEntitiesAsync(ct);

        await integrationEventService.AddAndSaveEventAsync(
            new DonationIntentReceivedIntegrationEvent(
                donationIntent.Id,
                donationIntent.CampaignId,
                donationIntent.DonorUserId,
                donationIntent.Amount,
                donationIntent.CorrelationId),
            ct);

        return donationIntent.Id;
    }
}
