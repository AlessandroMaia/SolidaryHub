namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentById;

internal sealed class GetDonationIntentByIdQueryValidator : AbstractValidator<GetDonationIntentByIdQuery>
{
    public GetDonationIntentByIdQueryValidator()
    {
        RuleFor(x => x.DonationIntentId)
            .GreaterThan(0).WithMessage("A intenção de doação é obrigatória");
    }
}
