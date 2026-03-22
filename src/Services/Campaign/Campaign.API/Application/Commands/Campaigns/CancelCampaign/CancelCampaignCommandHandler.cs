namespace Campaign.API.Application.Commands.Campaigns.CancelCampaign;

internal sealed class CancelCampaignCommandHandler(ICampaignRepository repository)
    : ICommandHandler<CancelCampaignCommand>
{
    public async Task Handle(CancelCampaignCommand command, CancellationToken ct)
    {
        var campaign = await repository.GetByIdAsync(command.CampaignId, ct)
            ?? throw new CampaignDomainException("Campanha não encontrada");

        campaign.Cancel(command.ChangedByUserId, command.Reason);

        repository.Update(campaign);
        await repository.UnitOfWork.SaveEntitiesAsync(ct);
    }
}
