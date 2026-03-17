using CampaignEntity = Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign;

namespace Campaign.Domain.AggregatesModel.CampaignAggregate.Repositories;

public interface ICampaignRepository : IRepository<CampaignEntity>
{
    Task<CampaignEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CampaignEntity?> GetByIdWithLedgerAsync(int id, CancellationToken ct = default);
    Task<List<CampaignEntity>> GetActiveAsync(CancellationToken ct = default);

    CampaignEntity Add(CampaignEntity campaign);
    CampaignEntity Update(CampaignEntity campaign);
}