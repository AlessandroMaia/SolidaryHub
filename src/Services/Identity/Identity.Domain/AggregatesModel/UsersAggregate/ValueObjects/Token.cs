namespace Identity.Domain.AggregatesModel.UsersAggregate.ValueObjects;

public class Token : ValueObject
{
    public string Value { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    protected Token() { }

    public Token(string value, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new IdentityDomainException("O valor do token é obrigatório");

        if (expiresAt <= DateTime.UtcNow)
            throw new IdentityDomainException("A expiração do token deve ser maior que o horário atual");

        Value = value;
        ExpiresAt = expiresAt;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return ExpiresAt;
    }

    public static implicit operator string(Token token) => token.Value;
}
