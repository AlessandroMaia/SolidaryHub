namespace Campaign.UnitTests.Domain.Entities;

public sealed class DonationLedgerEntryTests
{
    [Fact]
    public void Create_WithValidData_ShouldTrimSource()
    {
        var entry = DonationLedgerEntry.Create(1, 2, 3, 50m, " api ", "corr");

        entry.CampaignId.Should().Be(1);
        entry.DonationIntentId.Should().Be(2);
        entry.DonorUserId.Should().Be(3);
        entry.Amount.Should().Be(50m);
        entry.Source.Should().Be("api");
        entry.CorrelationId.Should().Be("corr");
    }

    [Fact]
    public void Create_WithInvalidAmount_ShouldThrow()
    {
        var act = () => DonationLedgerEntry.Create(1, 2, 3, 0m, "api");

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("O valor da doação deve ser maior que zero.");
    }

    [Fact]
    public void Create_WithEmptySource_ShouldThrow()
    {
        var act = () => DonationLedgerEntry.Create(1, 2, 3, 5m, "");

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("A origem do registro da doação é obrigatória.");
    }
}
