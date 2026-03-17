namespace Campaign.Domain.AggregatesModel.CampaignAggregate.ValueObjects;

public class CampaignPeriod : ValueObject
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    protected CampaignPeriod() { }

    public CampaignPeriod(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new CampaignDomainException("A data de término deve ser maior ou igual à data de início.");

        if (endDate.Date < DateTime.UtcNow.Date)
            throw new CampaignDomainException("A campanha não pode ser criada com data de término no passado.");

        StartDate = startDate;
        EndDate = endDate;
    }

    public bool IsExpired(DateTime referenceDate)
        => EndDate < referenceDate;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}
