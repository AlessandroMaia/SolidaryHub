namespace Campaign.Domain.AggregatesModel.CampaignAggregate.ValueObjects;

public class CampaignPeriod : ValueObject
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    protected CampaignPeriod() { }

    public CampaignPeriod(DateTime startDate, DateTime endDate)
    {
        EnsureUtc(startDate, "A data de inicio deve estar em UTC.");
        EnsureUtc(endDate, "A data de termino deve estar em UTC.");

        if (endDate < startDate)
            throw new CampaignDomainException("A data de termino deve ser maior ou igual a data de inicio.");

        if (endDate.Date < DateTime.UtcNow.Date)
            throw new CampaignDomainException("A campanha nao pode ser criada com data de termino no passado.");

        StartDate = startDate;
        EndDate = endDate;
    }

    public bool IsExpired(DateTime referenceDate)
    {
        EnsureUtc(referenceDate, "A data de referencia deve estar em UTC.");

        return EndDate < referenceDate;
    }

    private static void EnsureUtc(DateTime value, string message)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new CampaignDomainException(message);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}
