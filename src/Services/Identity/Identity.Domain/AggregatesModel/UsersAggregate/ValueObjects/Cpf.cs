using System.Text.RegularExpressions;

namespace Identity.Domain.AggregatesModel.UsersAggregate.ValueObjects;

public partial class Cpf : ValueObject
{
    public string Value { get; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new IdentityDomainException("CPF é obrigatório");

        var normalized = Normalize()
            .Replace(value, "");

        if (!HasValidFormat(value))
            throw new IdentityDomainException("Formato do CPF inválido");

        if (!HasValidDigits(normalized))
            throw new IdentityDomainException("CPF inválido");

        Value = normalized;
    }

    private static bool HasValidFormat(string cpf)
    {
        var trimmed = cpf.Trim();

        return CpfDigitsOnlyRegex().IsMatch(trimmed) ||
               CpfFormattedRegex().IsMatch(trimmed);
    }

    private static bool HasValidDigits(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        if (cpf.All(c => c == cpf[0]))
            return false;

        var numbers = cpf.Select(c => c - '0').ToArray();

        var firstDigit = CalculateDigit(numbers, 9, 10);
        var secondDigit = CalculateDigit(numbers, 10, 11);

        return numbers[9] == firstDigit && numbers[10] == secondDigit;
    }

    private static int CalculateDigit(int[] numbers, int length, int weightStart)
    {
        var sum = 0;

        for (int i = 0; i < length; i++)
            sum += numbers[i] * (weightStart - i);

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^\d{11}$")]
    private static partial Regex CpfDigitsOnlyRegex();

    [GeneratedRegex(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")]
    private static partial Regex CpfFormattedRegex();

    [GeneratedRegex(@"\D")]
    private static partial Regex Normalize();
}