namespace Campaign.API.Application.Commands.Campaigns.CreateCampaign;

public record CreateCampaignCommand(
    int UserId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal FinancialGoalAmount,
    Guid RequestId = default) : IIdempotentCommand<int>;
