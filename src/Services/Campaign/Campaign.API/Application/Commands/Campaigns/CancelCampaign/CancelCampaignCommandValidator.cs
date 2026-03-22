namespace Campaign.API.Application.Commands.Campaigns.CancelCampaign;

internal sealed class CancelCampaignCommandValidator : AbstractValidator<CancelCampaignCommand>
{
    public CancelCampaignCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .GreaterThan(0).WithMessage("A campanha é obrigatória");

        RuleFor(x => x.ChangedByUserId)
            .GreaterThan(0).WithMessage("O gestor informado é inválido");
    }
}
