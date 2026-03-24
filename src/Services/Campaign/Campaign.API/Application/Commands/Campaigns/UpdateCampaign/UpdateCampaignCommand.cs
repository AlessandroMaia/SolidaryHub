namespace Campaign.API.Application.Commands.Campaigns.UpdateCampaign;

public record UpdateCampaignCommand(
    int CampaignId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal FinancialGoalAmount,
    Guid RequestId = default) : IIdempotentCommand;
