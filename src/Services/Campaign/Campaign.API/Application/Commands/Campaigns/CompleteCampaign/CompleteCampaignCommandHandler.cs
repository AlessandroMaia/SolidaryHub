namespace Campaign.API.Application.Commands.Campaigns.CompleteCampaign;

internal sealed class CompleteCampaignCommandHandler(ICampaignRepository repository)
    : ICommandHandler<CompleteCampaignCommand>
{
    public async Task Handle(CompleteCampaignCommand command, CancellationToken ct)
    {
        var campaign = await repository.GetByIdAsync(command.CampaignId, ct)
            ?? throw new CampaignDomainException("Campanha não encontrada");

        campaign.Complete(command.ChangedByUserId, command.Reason);

        repository.Update(campaign);
        await repository.UnitOfWork.SaveEntitiesAsync(ct);
    }
}
