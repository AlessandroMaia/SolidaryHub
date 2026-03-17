namespace Campaign.Infrastructure.Repositories;

public class DonationIntentRepository(CampaignContext context) : IDonationIntentRepository
{
    private readonly CampaignContext _context = context;
    public IUnitOfWork UnitOfWork => _context;

    public DonationIntent Add(DonationIntent donationIntent)
        => _context.DonationIntents.Add(donationIntent).Entity;

    public DonationIntent Update(DonationIntent donationIntent)
        => _context.DonationIntents.Update(donationIntent).Entity;

    public async Task<DonationIntent?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.DonationIntents
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<DonationIntent?> GetByMessageIdAsync(string messageId, CancellationToken ct = default)
        => await _context.DonationIntents
            .FirstOrDefaultAsync(u => u.MessageId == messageId, ct);

    public async Task<List<DonationIntent>> GetPendingAsync(CancellationToken ct = default)
        => await _context.DonationIntents
            .Where(u => u.Status == DonationIntentStatus.Pending)
            .ToListAsync(ct);
}
