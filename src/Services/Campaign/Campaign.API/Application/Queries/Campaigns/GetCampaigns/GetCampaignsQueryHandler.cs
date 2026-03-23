namespace Campaign.API.Application.Queries.Campaigns.GetCampaigns;

internal sealed class GetCampaignsQueryHandler(CampaignContext context)
    : IQueryHandler<GetCampaignsQuery, PagedResponse<CampaignListItemViewModel>>
{
    public async Task<PagedResponse<CampaignListItemViewModel>> Handle(
        GetCampaignsQuery query,
        CancellationToken ct)
    {
        var campaignsQuery = context.Campaigns
            .AsNoTracking()
            .Where(c => c.Status == CampaignStatus.Active);

        var totalCount = await campaignsQuery.CountAsync(ct);

        var campaigns = await campaignsQuery
            .OrderByDescending(u => u.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c =>
                new CampaignListItemViewModel(
                    c.Id,
                    c.Title,
                    c.FinancialGoalAmount,
                    c.Total.TotalAmountRaised,
                    c.Status.ToString(),
                    c.StartDate,
                    c.EndDate))
            .ToListAsync(ct);

        return new PagedResponse<CampaignListItemViewModel>(campaigns, query.Page, query.PageSize, totalCount);
    }
}
