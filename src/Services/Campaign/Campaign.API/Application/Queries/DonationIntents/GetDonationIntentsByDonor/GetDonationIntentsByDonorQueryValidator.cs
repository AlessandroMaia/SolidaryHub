namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentsByDonor;

internal sealed class GetDonationIntentsByDonorQueryValidator : AbstractValidator<GetDonationIntentsByDonorQuery>
{
    public GetDonationIntentsByDonorQueryValidator()
    {
        RuleFor(x => x.DonorUserId)
            .GreaterThan(0).WithMessage("O doador é obrigatório");
    }
}
