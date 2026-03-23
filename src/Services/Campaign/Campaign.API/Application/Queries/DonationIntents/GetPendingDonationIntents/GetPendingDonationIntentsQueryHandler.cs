namespace Campaign.API.Application.Queries.DonationIntents.GetPendingDonationIntents;

internal sealed class GetPendingDonationIntentsQueryHandler(CampaignContext context)
    : IQueryHandler<GetPendingDonationIntentsQuery, PagedResponse<DonationIntentListItemViewModel>>
{
    public async Task<PagedResponse<DonationIntentListItemViewModel>> Handle(
        GetPendingDonationIntentsQuery query,
        CancellationToken ct)
    {
        var donationsQuery = context.DonationIntents
            .AsNoTracking()
            .Where(d => d.Status == DonationIntentStatus.Pending);

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
