namespace Identity.Domain.AggregatesModel.UsersAggregate.ValueObjects;

public class Password : ValueObject
{
    public string Hash { get; private set; } = null!;

    protected Password() { }

    private Password(string hash)
    {
        Hash = hash;
    }

    public static Password FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new IdentityDomainException("O hash de senha é obrigatório");

        return new Password(hash);
    }

    public static void ValidateStrength(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new IdentityDomainException("A senha é obrigatória");

        if (plainPassword.Length < 8)
            throw new IdentityDomainException("A senha deve conter no mínimo 8 caracteres");

        if (!plainPassword.Any(char.IsUpper))
            throw new IdentityDomainException("A senha deve conter no mínimo uma letra maiúscula");

        if (!plainPassword.Any(char.IsLower))
            throw new IdentityDomainException("A senha deve conter no mínimo uma letra minúscula");

        if (!plainPassword.Any(char.IsDigit))
            throw new IdentityDomainException("A senha deve conter no mínimo um número");

        if (!plainPassword.Any(c => !char.IsLetterOrDigit(c)))
            throw new IdentityDomainException("A senha deve conter no mínimo um caractere especial");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
    }

    public override string ToString() => "********";
}
