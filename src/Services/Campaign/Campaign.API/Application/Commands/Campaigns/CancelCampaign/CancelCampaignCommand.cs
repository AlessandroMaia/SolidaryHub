namespace Campaign.API.Application.Commands.Campaigns.CancelCampaign;

public sealed record CancelCampaignCommand(
    int CampaignId,
    int ChangedByUserId,
    string? Reason,
    Guid RequestId = default) : IIdempotentCommand;
