namespace Campaign.API.Application.Commands.DonationIntents.FailDonationIntentProcessing;

internal sealed class FailDonationIntentProcessingCommandHandler(
    IDonationIntentRepository donationIntentRepository)
    : ICommandHandler<FailDonationIntentProcessingCommand>
{
    public async Task Handle(FailDonationIntentProcessingCommand command, CancellationToken ct)
    {
        var donationIntent = await donationIntentRepository.GetByIdAsync(command.DonationIntentId, ct)
            ?? throw new CampaignDomainException("Intenção de doação não encontrada");

        donationIntent.MarkAsFailed(command.WorkerName, command.Reason, sendToDeadLetter: true);

        donationIntentRepository.Update(donationIntent);
        await donationIntentRepository.UnitOfWork.SaveEntitiesAsync(ct);
    }
}
