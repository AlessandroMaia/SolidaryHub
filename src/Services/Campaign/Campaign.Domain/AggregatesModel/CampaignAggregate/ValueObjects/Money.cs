namespace Campaign.Domain.AggregatesModel.CampaignAggregate.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;

    protected Money() { }

    public Money(decimal amount, string currency)
    {
        if (amount <= 0)
            throw new CampaignDomainException("O valor monetário deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new CampaignDomainException("A moeda é obrigatória.");

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}