namespace Campaign.UnitTests.Domain.ValueObjects;

public sealed class CampaignTotalTests
{
    [Fact]
    public void Create_ShouldInitializeZeroTotals()
    {
        var total = CampaignTotal.Create();

        total.TotalAmountRaised.Should().Be(0);
        total.TotalDonationsCount.Should().Be(0);
        total.LastDonationAt.Should().BeNull();
    }

    [Fact]
    public void ApplyDonation_WithPositiveAmount_ShouldReturnUpdatedTotal()
    {
        var total = CampaignTotal.Create();
        var processedAt = DateTime.UtcNow;

        var updated = total.ApplyDonation(25m, processedAt);

        updated.TotalAmountRaised.Should().Be(25m);
        updated.TotalDonationsCount.Should().Be(1);
        updated.LastDonationAt.Should().Be(processedAt);
        updated.UpdatedAt.Should().BeOnOrAfter(total.UpdatedAt);
    }

    [Fact]
    public void ApplyDonation_WithInvalidAmount_ShouldThrow()
    {
        var total = CampaignTotal.Create();

        var act = () => total.ApplyDonation(0m, DateTime.UtcNow);

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("O valor da doação deve ser maior que zero.");
    }
[Fact]
    public void Equality_WithSameComponents_ShouldBeEqual()
    {
        var processedAt = DateTime.UtcNow;
        var left = CampaignTotal.Create().ApplyDonation(10m, processedAt);
        var right = CampaignTotal.Create().ApplyDonation(10m, processedAt);
        typeof(CampaignTotal).GetProperty(nameof(CampaignTotal.UpdatedAt))!
            .SetValue(right, left.UpdatedAt);

        left.Should().Be(right);
    }
}
