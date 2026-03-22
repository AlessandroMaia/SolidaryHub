namespace Campaign.API.Application.Commands.DonationIntents.ProcessDonationIntent;

internal sealed class ProcessDonationIntentCommandHandler(
    IDonationIntentRepository donationIntentRepository,
    ICampaignRepository campaignRepository)
    : ICommandHandler<ProcessDonationIntentCommand>
{
    public async Task Handle(ProcessDonationIntentCommand command, CancellationToken ct)
    {
        var donationIntent = await donationIntentRepository.GetByIdAsync(command.DonationIntentId, ct)
            ?? throw new CampaignDomainException("Intenção de doação não encontrada");

        var campaign = await campaignRepository.GetByIdWithLedgerAsync(donationIntent.CampaignId, ct)
            ?? throw new CampaignDomainException("Campanha não encontrada");

        donationIntent.MarkAsProcessing(command.WorkerName);

        campaign.ApplyDonation(
            donationIntent.Id,
            donationIntent.DonorUserId,
            donationIntent.Amount,
            donationIntent.Source,
            donationIntent.CorrelationId);

        donationIntent.MarkAsProcessed(command.WorkerName);

        donationIntentRepository.Update(donationIntent);
        campaignRepository.Update(campaign);

        await donationIntentRepository.UnitOfWork.SaveEntitiesAsync(ct);
    }
}
