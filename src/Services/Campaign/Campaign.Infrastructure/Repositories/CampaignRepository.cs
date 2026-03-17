namespace Campaign.Infrastructure.Repositories;

public class CampaignRepository(CampaignContext context): ICampaignRepository
{
    private readonly CampaignContext _context = context;
    public IUnitOfWork UnitOfWork => _context;

    public CampaignEntity Add(CampaignEntity campaign)
        => _context.Campaigns.Add(campaign).Entity;

    public CampaignEntity Update(CampaignEntity campaign)
        => _context.Campaigns.Update(campaign).Entity;

    public async Task<List<CampaignEntity>> GetActiveAsync(CancellationToken ct = default)
        => await _context.Campaigns
            .Where(c => c.Status == CampaignStatus.Active)
            .ToListAsync(ct);

    public async Task<CampaignEntity?> GetByIdAsync(int id, CancellationToken ct = default) 
        => await _context.Campaigns
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<CampaignEntity?> GetByIdWithLedgerAsync(int id, CancellationToken ct = default)
        => await _context.Campaigns
            .Include(c => c.DonationLedger)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
}
