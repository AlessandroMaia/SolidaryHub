namespace Campaign.API.Application.Queries.Campaigns.GetCampaignPublicPanel;

public sealed record GetCampaignPublicPanelQuery(int Page = 1, int PageSize = 10) 
    : IQuery<PagedResponse<CampaignPublicPanelViewModel>>;
