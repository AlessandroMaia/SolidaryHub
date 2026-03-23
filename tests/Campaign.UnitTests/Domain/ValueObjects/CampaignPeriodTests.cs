namespace Campaign.UnitTests.Domain.ValueObjects;

public sealed class CampaignPeriodTests
{
    [Fact]
    public void Constructor_WithValidDates_ShouldCreatePeriod()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(1);

        var period = new CampaignPeriod(start, end);

        period.StartDate.Should().Be(start);
        period.EndDate.Should().Be(end);
    }

    [Fact]
    public void Constructor_WithEndDateBeforeStartDate_ShouldThrow()
    {
        var act = () => new CampaignPeriod(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(-1));

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("A data de término deve ser maior ou igual à data de início.");
    }

    [Fact]
    public void Constructor_WithEndDateInPastDate_ShouldThrow()
    {
        var start = DateTime.UtcNow.AddDays(-2);
        var end = DateTime.UtcNow.AddDays(-1);

        var act = () => new CampaignPeriod(start, end);

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("A campanha não pode ser criada com data de término no passado.");
    }

    [Fact]
    public void IsExpired_ShouldRespectReferenceDate()
    {
        var period = new CampaignPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddMinutes(1));

        period.IsExpired(DateTime.UtcNow).Should().BeFalse();
        period.IsExpired(DateTime.UtcNow.AddDays(1)).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithSameDates_ShouldBeEqual()
    {
        var start = DateTime.UtcNow.Date.AddDays(1);
        var end = start.AddDays(5);

        var left = new CampaignPeriod(start, end);
        var right = new CampaignPeriod(start, end);

        left.Should().Be(right);
    }
}
