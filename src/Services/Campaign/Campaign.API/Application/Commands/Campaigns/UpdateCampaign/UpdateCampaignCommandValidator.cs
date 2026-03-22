namespace Campaign.API.Application.Commands.Campaigns.UpdateCampaign;

public sealed class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .GreaterThan(0).WithMessage("ID da campanha é obrigatório");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória");

        RuleFor(x => x.FinancialGoalAmount)
            .GreaterThan(0).WithMessage("A meta financeira deve ser maior que zero");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("A data de término deve ser maior ou igual à data de início");
    }
}
