namespace Campaign.API.Application.Commands.DonationIntents.RejectDonationIntent;

internal sealed class RejectDonationIntentCommandHandler(
    IDonationIntentRepository donationIntentRepository)
        : ICommandHandler<RejectDonationIntentCommand>
{
    public async Task Handle(RejectDonationIntentCommand command, CancellationToken ct)
    {
        var donationIntent = await donationIntentRepository.GetByIdAsync(command.DonationIntentId, ct)
            ?? throw new CampaignDomainException("Intenção de doação não encontrada");

        donationIntent.Reject(command.Reason);

        donationIntentRepository.Update(donationIntent);
        await donationIntentRepository.UnitOfWork.SaveEntitiesAsync(ct);
    }
}
