namespace Campaign.API.Application.Queries.Campaigns.GetActiveCampaigns;

public sealed record GetActiveCampaignsQuery(int Page = 1, int PageSize = 10) 
    : IQuery<PagedResponse<CampaignListItemViewModel>>;
