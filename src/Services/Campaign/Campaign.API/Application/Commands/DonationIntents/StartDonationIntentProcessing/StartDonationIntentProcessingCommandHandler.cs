namespace Campaign.API.Application.Commands.DonationIntents.StartDonationIntentProcessing;

internal sealed class StartDonationIntentProcessingCommandHandler(
    IDonationIntentRepository donationIntentRepository)
    : ICommandHandler<StartDonationIntentProcessingCommand>
{
    public async Task Handle(StartDonationIntentProcessingCommand command, CancellationToken ct)
    {
        var donationIntent = await donationIntentRepository.GetByIdAsync(command.DonationIntentId, ct)
            ?? throw new CampaignDomainException("Intenção de doação não encontrada");

        donationIntent.MarkAsProcessing(command.WorkerName);

        donationIntentRepository.Update(donationIntent);
        await donationIntentRepository.UnitOfWork.SaveEntitiesAsync(ct);
    }
}
