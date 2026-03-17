namespace Campaign.Domain.AggregatesModel.CampaignAggregate.Entities;

public class DonationLedgerEntry : Entity
{
    public int CampaignId { get; private set; }
    public Guid DonationIntentId { get; private set; }
    public int DonorUserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Source { get; private set; } = null!;
    public string? CorrelationId { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    protected DonationLedgerEntry() { }

    private DonationLedgerEntry(
        int campaignId,
        Guid donationIntentId,
        int donorUserId,
        decimal amount,
        string source,
        string? correlationId)
    {
        CampaignId = campaignId;
        DonationIntentId = donationIntentId;
        DonorUserId = donorUserId;
        Amount = amount;
        Source = source;
        CorrelationId = correlationId;
        ProcessedAt = DateTime.UtcNow;
    }

    public static DonationLedgerEntry Create(
        int campaignId,
        Guid donationIntentId,
        int donorUserId,
        decimal amount,
        string source,
        string? correlationId = null)
    {
        if (amount <= 0)
            throw new CampaignDomainException("O valor da doação deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(source))
            throw new CampaignDomainException("A origem do registro da doação é obrigatória.");

        return new DonationLedgerEntry(
            campaignId,
            donationIntentId,
            donorUserId,
            amount,
            source.Trim(),
            correlationId);
    }
}
