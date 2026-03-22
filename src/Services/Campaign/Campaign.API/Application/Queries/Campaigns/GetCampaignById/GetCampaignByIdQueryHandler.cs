namespace Campaign.API.Application.Queries.Campaigns.GetCampaignById;

internal sealed class GetCampaignByIdQueryHandler(CampaignContext context)
    : IQueryHandler<GetCampaignByIdQuery, CampaignDetailsViewModel?>
{
    public async Task<CampaignDetailsViewModel?> Handle(
        GetCampaignByIdQuery query,
        CancellationToken ct)
    {
        return await context.Campaigns
            .AsNoTracking()
            .Where(c => c.Id == query.CampaignId)
            .Select(c => 
                new CampaignDetailsViewModel(
                    c.Id,
                    c.Title,
                    c.Description,
                    c.StartDate,
                    c.EndDate,
                    c.FinancialGoalAmount,
                    c.Total.TotalAmountRaised,
                    c.Total.TotalDonationsCount,
                    c.Status.ToString(),
                    c.CreatedAt,
                    c.UpdatedAt))
            .FirstOrDefaultAsync(ct);
    }
}
