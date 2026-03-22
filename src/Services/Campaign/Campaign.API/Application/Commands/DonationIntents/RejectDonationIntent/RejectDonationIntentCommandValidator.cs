namespace Campaign.API.Application.Commands.DonationIntents.RejectDonationIntent;

internal sealed class RejectDonationIntentCommandValidator : AbstractValidator<RejectDonationIntentCommand>
{
    public RejectDonationIntentCommandValidator()
    {
        RuleFor(x => x.DonationIntentId)
            .GreaterThan(0).WithMessage("A intenção de doação é obrigatória");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("O motivo da rejeição é obrigatório")
            .MaximumLength(300);
    }
}
