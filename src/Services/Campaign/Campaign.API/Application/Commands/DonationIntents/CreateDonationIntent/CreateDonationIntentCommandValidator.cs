namespace Campaign.API.Application.Commands.DonationIntents.CreateDonationIntent;

internal sealed class CreateDonationIntentCommandValidator : AbstractValidator<CreateDonationIntentCommand>
{
    public CreateDonationIntentCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .GreaterThan(0).WithMessage("A campanha é obrigatória");

        RuleFor(x => x.DonorUserId)
            .GreaterThan(0).WithMessage("O doador é obrigatório");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("O valor da doação deve ser maior que zero");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("A moeda é obrigatória")
            .MaximumLength(10);

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("A origem é obrigatória")
            .MaximumLength(50);
    }
}
