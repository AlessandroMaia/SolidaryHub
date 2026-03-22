namespace Campaign.API.Application.Commands.Campaigns.UpdateCampaign;

public sealed class UpdateCampaignCommandHandler(ICampaignRepository repository)
    : ICommandHandler<UpdateCampaignCommand>
{
    public async Task Handle(UpdateCampaignCommand command, CancellationToken ct)
    {
        var campaign = await repository.GetByIdAsync(command.CampaignId, ct)
            ?? throw new CampaignDomainException($"Campanha com ID {command.CampaignId} não encontrada.");

        campaign.UpdateDetails(
            command.Title, 
            command.Description, 
            command.StartDate, 
            command.EndDate, 
            command.FinancialGoalAmount);

        repository.Update(campaign);

        await repository.UnitOfWork.SaveChangesAsync(ct);
    }
}
