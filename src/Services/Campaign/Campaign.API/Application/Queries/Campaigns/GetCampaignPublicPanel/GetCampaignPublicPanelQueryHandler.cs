namespace Campaign.API.Application.Queries.Campaigns.GetCampaignPublicPanel;

internal sealed class GetCampaignPublicPanelQueryHandler(CampaignContext context)
    : IQueryHandler<GetCampaignPublicPanelQuery, PagedResponse<CampaignPublicPanelViewModel>>
{
    public async Task<PagedResponse<CampaignPublicPanelViewModel>> Handle(
        GetCampaignPublicPanelQuery query,
        CancellationToken ct)
    {
        var campaignsQuery = context.Campaigns
            .AsNoTracking()
            .Where(c => c.Status == CampaignStatus.Active);

        var totalCount = await campaignsQuery.CountAsync(ct);

        var campaigns = await campaignsQuery
            .OrderByDescending(u => u.EndDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c =>
                new CampaignPublicPanelViewModel(
                    c.Id,
                    c.Title,
                    c.FinancialGoalAmount,
                    c.Total.TotalAmountRaised))
            .ToListAsync(ct);

        return new PagedResponse<CampaignPublicPanelViewModel>(campaigns, totalCount, query.Page, query.PageSize);
    }
}
