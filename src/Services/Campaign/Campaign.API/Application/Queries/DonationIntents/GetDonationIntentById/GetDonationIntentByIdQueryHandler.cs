namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentById;

internal sealed class GetDonationIntentByIdQueryHandler(CampaignContext context)
    : IQueryHandler<GetDonationIntentByIdQuery, DonationIntentDetailsViewModel?>
{
    public async Task<DonationIntentDetailsViewModel?> Handle(
        GetDonationIntentByIdQuery query,
        CancellationToken ct)
    {
        return await context.DonationIntents
            .AsNoTracking()
            .Where(d => d.Id == query.DonationIntentId)
            .Select(d => 
                new DonationIntentDetailsViewModel(
                    d.Id,
                    d.CampaignId,
                    d.DonorUserId,
                    d.Amount,
                    d.Currency,
                    d.Status.ToString(),
                    d.Source,
                    d.CorrelationId,
                    d.MessageId,
                    d.RequestedAt,
                    d.ValidatedAt,
                    d.RejectedAt,
                    d.ProcessedAt,
                    d.RejectionReason))
            .FirstOrDefaultAsync(ct);
    }
}
