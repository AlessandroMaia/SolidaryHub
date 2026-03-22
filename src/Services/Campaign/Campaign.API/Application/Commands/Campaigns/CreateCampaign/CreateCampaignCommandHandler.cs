namespace Campaign.API.Application.Commands.Campaigns.CreateCampaign;

using CampaignEntity = Domain.AggregatesModel.CampaignAggregate.Entities.Campaign;

public sealed class CreateCampaignCommandHandler(ICampaignRepository repository)
    : ICommandHandler<CreateCampaignCommand, int>
{
    public async Task<int> Handle(CreateCampaignCommand command, CancellationToken ct)
    {
        var campaign = CampaignEntity.Create(
            command.Title,
            command.Description,
            command.StartDate,
            command.EndDate,
            command.FinancialGoalAmount,
            command.UserId);

        repository.Add(campaign);

        await repository.UnitOfWork.SaveEntitiesAsync(ct);

        return campaign.Id;
    }
}
