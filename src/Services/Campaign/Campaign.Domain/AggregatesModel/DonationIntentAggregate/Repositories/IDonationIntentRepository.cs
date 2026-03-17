namespace Campaign.Domain.AggregatesModel.DonationIntentAggregate.Repositories;

public interface IDonationIntentRepository : IRepository<DonationIntent>
{
    Task<DonationIntent?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<DonationIntent?> GetByMessageIdAsync(string messageId, CancellationToken ct = default);
    Task<List<DonationIntent>> GetPendingAsync(CancellationToken ct = default);

    DonationIntent Add(DonationIntent donationIntent);
    DonationIntent Update(DonationIntent donationIntent);
}
