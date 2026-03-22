namespace Campaign.API.Application.Queries.Campaigns.GetCampaigns;

public sealed record GetCampaignsQuery(int Page = 1, int PageSize = 10) 
    : IQuery<PagedResponse<CampaignListItemViewModel>>;