namespace Campaign.API.Application.Commands.DonationIntents.ProcessDonationIntent;

internal sealed class ProcessDonationIntentCommandValidator : AbstractValidator<ProcessDonationIntentCommand>
{
    public ProcessDonationIntentCommandValidator()
    {
        RuleFor(x => x.DonationIntentId)
            .GreaterThan(0).WithMessage("A intenção de doação é obrigatória");

        RuleFor(x => x.WorkerName)
            .NotEmpty().WithMessage("O nome do worker é obrigatório")
            .MaximumLength(100);
    }
}
