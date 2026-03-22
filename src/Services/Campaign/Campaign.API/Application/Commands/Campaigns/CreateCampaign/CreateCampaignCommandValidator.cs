namespace Campaign.API.Application.Commands.Campaigns.CreateCampaign;

public sealed class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória");

        RuleFor(x => x.FinancialGoalAmount)
            .GreaterThan(0).WithMessage("A meta financeira deve ser maior que zero");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("O gestor informado é inválido");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("A data de término deve ser maior ou igual à data de início");
    }
}
