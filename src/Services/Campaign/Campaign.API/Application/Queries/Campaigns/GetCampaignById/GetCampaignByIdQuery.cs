namespace Campaign.API.Application.Queries.Campaigns.GetCampaignById;

public sealed record GetCampaignByIdQuery(int CampaignId) 
    : IQuery<CampaignDetailsViewModel?>;