namespace Campaign.API.Application.Commands.Campaigns.CompleteCampaign;

internal sealed class CompleteCampaignCommandValidator : AbstractValidator<CompleteCampaignCommand>
{
    public CompleteCampaignCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .GreaterThan(0).WithMessage("A campanha é obrigatória");

        RuleFor(x => x.ChangedByUserId)
            .GreaterThan(0).WithMessage("O gestor informado é inválido");
    }
}
