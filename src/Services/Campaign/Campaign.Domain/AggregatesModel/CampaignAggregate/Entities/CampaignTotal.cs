namespace Campaign.Domain.AggregatesModel.CampaignAggregate.Entities;

public class CampaignTotal : Entity
{
    public decimal TotalAmountRaised { get; private set; }
    public int TotalDonationsCount { get; private set; }
    public DateTime? LastDonationAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private CampaignTotal()
    {
        TotalAmountRaised = 0;
        TotalDonationsCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public static CampaignTotal Create() => new();

    public void ApplyDonation(decimal amount, DateTime processedAt)
    {
        if (amount <= 0)
            throw new CampaignDomainException("O valor da doação deve ser maior que zero.");

        TotalAmountRaised += amount;
        TotalDonationsCount++;
        LastDonationAt = processedAt;
        UpdatedAt = DateTime.UtcNow;
    }
}
