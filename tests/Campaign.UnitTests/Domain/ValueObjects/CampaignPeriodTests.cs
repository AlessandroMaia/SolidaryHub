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
            .WithMessage("A data de termino deve ser maior ou igual a data de inicio.");
    }

    [Fact]
    public void Constructor_WithEndDateInPastDate_ShouldThrow()
    {
        var start = DateTime.UtcNow.AddDays(-2);
        var end = DateTime.UtcNow.AddDays(-1);

        var act = () => new CampaignPeriod(start, end);

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("A campanha nao pode ser criada com data de termino no passado.");
    }

    [Fact]
    public void Constructor_WithNonUtcStartDate_ShouldThrow()
    {
        var start = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local);
        var end = DateTime.UtcNow.AddDays(1);

        var act = () => new CampaignPeriod(start, end);

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("A data de inicio deve estar em UTC.");
    }

    [Fact]
    public void IsExpired_ShouldRespectReferenceDate()
    {
        var period = new CampaignPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddMinutes(1));

        period.IsExpired(DateTime.UtcNow).Should().BeFalse();
        period.IsExpired(DateTime.UtcNow.AddDays(1)).Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WithNonUtcReferenceDate_ShouldThrow()
    {
        var period = new CampaignPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddMinutes(1));
        var referenceDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local);

        var act = () => period.IsExpired(referenceDate);

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("A data de referencia deve estar em UTC.");
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
