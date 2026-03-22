namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentsByCampaign;

internal sealed class GetDonationIntentsByCampaignQueryValidator : AbstractValidator<GetDonationIntentsByCampaignQuery>
{
    public GetDonationIntentsByCampaignQueryValidator()
    {
        RuleFor(x => x.CampaignId)
            .GreaterThan(0).WithMessage("A campanha é obrigatória");
    }
}
