namespace Campaign.API.Application.Commands.Campaigns.CompleteCampaign;

public sealed record CompleteCampaignCommand(
    int CampaignId,
    int ChangedByUserId,
    string? Reason,
    Guid RequestId = default) : IIdempotentCommand;
