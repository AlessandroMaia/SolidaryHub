namespace Campaign.Domain.AggregatesModel.CampaignAggregate.Entities;

public class CampaignStatusHistory : Entity
{
    public int CampaignId { get; private set; }
    public CampaignStatus? OldStatus { get; private set; }
    public CampaignStatus NewStatus { get; private set; }
    public string Reason { get; private set; } = null!;
    public int ChangedByUserId { get; private set; }
    public DateTime ChangedAt { get; private set; }

    protected CampaignStatusHistory() { }

    private CampaignStatusHistory(
        int campaignId,
        CampaignStatus? oldStatus,
        CampaignStatus newStatus,
        string reason,
        int changedByUserId)
    {
        CampaignId = campaignId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
        ChangedByUserId = changedByUserId;
        ChangedAt = DateTime.UtcNow;
    }

    public static CampaignStatusHistory Create(
        int campaignId,
        CampaignStatus? oldStatus,
        CampaignStatus newStatus,
        string reason,
        int changedByUserId)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new CampaignDomainException("O motivo da alteração de status é obrigatório.");

        return new CampaignStatusHistory(campaignId, oldStatus, newStatus, reason.Trim(), changedByUserId);
    }
}