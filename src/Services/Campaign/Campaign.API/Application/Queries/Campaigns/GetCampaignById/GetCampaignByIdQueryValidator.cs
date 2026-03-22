namespace Campaign.API.Application.Queries.Campaigns.GetCampaignById;

internal sealed class GetCampaignByIdQueryValidator : AbstractValidator<GetCampaignByIdQuery>
{
    public GetCampaignByIdQueryValidator()
    {
        RuleFor(x => x.CampaignId)
            .GreaterThan(0).WithMessage("A campanha é obrigatória");
    }
}
