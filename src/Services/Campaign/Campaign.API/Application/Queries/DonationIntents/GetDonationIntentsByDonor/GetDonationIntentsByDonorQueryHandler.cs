namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentsByDonor;

internal sealed class GetDonationIntentsByDonorQueryHandler(CampaignContext context)
    : IQueryHandler<GetDonationIntentsByDonorQuery, PagedResponse<DonationIntentListItemViewModel>>
{
    public async Task<PagedResponse<DonationIntentListItemViewModel>> Handle(
        GetDonationIntentsByDonorQuery query,
        CancellationToken ct)
    {
        var donationsQuery = context.DonationIntents
            .AsNoTracking()
            .Where(d => d.DonorUserId == query.DonorUserId);

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

        return new PagedResponse<DonationIntentListItemViewModel>(donations, query.Page, query.PageSize, totalCount);
    }
}
