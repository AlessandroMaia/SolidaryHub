namespace Identity.Domain.AggregatesModel.UsersAggregate.ValueObjects;

public class PersonName : ValueObject
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string FullName => $"{FirstName} {LastName}";

    protected PersonName() { }

    public PersonName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new IdentityDomainException("O nome é obrigatório");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new IdentityDomainException("O sobrenome é obrigatório");

        if (firstName.Length > 100)
            throw new IdentityDomainException("O nome deve conter no máximo 100 caracteres");

        if (lastName.Length > 100)
            throw new IdentityDomainException("O sobrenome deve conter no máximo 100 caracteres");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString() => FullName;
}