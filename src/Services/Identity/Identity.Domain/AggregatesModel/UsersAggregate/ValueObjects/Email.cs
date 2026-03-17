using System.Net.Mail;

namespace Identity.Domain.AggregatesModel.UsersAggregate.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; } = null!;

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new IdentityDomainException("O e-mail é obrigatório");

        var normalized = value.Trim().ToLowerInvariant();

        if (!IsValidEmail(normalized))
            throw new IdentityDomainException("O e-mail informado é inválido");

        Value = normalized;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;
}
