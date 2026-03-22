namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentsByCampaign;

internal sealed class GetDonationIntentsByCampaignQueryHandler(CampaignContext context)
    : IQueryHandler<GetDonationIntentsByCampaignQuery, PagedResponse<DonationIntentListItemViewModel>>
{
    public async Task<PagedResponse<DonationIntentListItemViewModel>> Handle(
        GetDonationIntentsByCampaignQuery query,
        CancellationToken ct)
    {
        var donationsQuery = context.DonationIntents
            .AsNoTracking()
            .Where(d => d.CampaignId == query.CampaignId);

        var totalCount = await donationsQuery.CountAsync(ct);

        var donations = await donationsQuery
            .OrderByDescending(d => d.RequestedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d =>
                new DonationIntentListItemViewModel(
                    d.Id,
                    d.CampaignId,
                    d.DonorUserId,
                    d.Amount,
                    d.Currency,
                    d.Status.ToString(),
                    d.RequestedAt,
                    d.ProcessedAt))
            .ToListAsync(ct);

        return new PagedResponse<DonationIntentListItemViewModel>(donations, totalCount, query.Page, query.PageSize);
    }
}
