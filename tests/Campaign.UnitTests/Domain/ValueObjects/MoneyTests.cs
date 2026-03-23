namespace Campaign.UnitTests.Domain.ValueObjects;

public sealed class MoneyTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldNormalizeCurrency()
    {
        var money = new Money(100m, " brl ");

        money.Amount.Should().Be(100m);
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Constructor_WithNonPositiveAmount_ShouldThrow()
    {
        var act = () => new Money(0m, "BRL");

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("O valor monetário deve ser maior que zero.");
    }

    [Fact]
    public void Constructor_WithEmptyCurrency_ShouldThrow()
    {
        var act = () => new Money(10m, "");

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("A moeda é obrigatória.");
    }

    [Fact]
    public void Equality_WithSameComponents_ShouldBeEqual()
    {
        var left = new Money(50m, "BRL");
        var right = new Money(50m, "brl");

        left.Should().Be(right);
    }
}
