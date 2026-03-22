namespace Campaign.Domain.AggregatesModel.CampaignAggregate.ValueObjects;

public class CampaignTotal : ValueObject
{
    public decimal TotalAmountRaised { get; private set; }
    public int TotalDonationsCount { get; private set; }
    public DateTime? LastDonationAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private CampaignTotal(
        decimal totalAmountRaised,
        int totalDonationsCount,
        DateTime? lastDonationAt,
        DateTime updatedAt)
    {
        TotalAmountRaised = totalAmountRaised;
        TotalDonationsCount = totalDonationsCount;
        LastDonationAt = lastDonationAt;
        UpdatedAt = updatedAt;
    }

    public static CampaignTotal Create()
        => new(0, 0, null, DateTime.UtcNow);

    public CampaignTotal ApplyDonation(decimal amount, DateTime processedAt)
    {
        if (amount <= 0)
            throw new CampaignDomainException("O valor da doação deve ser maior que zero.");

        return new CampaignTotal(
            TotalAmountRaised + amount,
            TotalDonationsCount + 1,
            processedAt,
            DateTime.UtcNow);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalAmountRaised;
        yield return TotalDonationsCount;
        yield return UpdatedAt;
    }
}
