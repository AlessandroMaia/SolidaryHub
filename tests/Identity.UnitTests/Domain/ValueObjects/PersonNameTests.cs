namespace Identity.UnitTests.Domain.ValueObjects;

public sealed class PersonNameTests
{
    [Fact]
    public void Constructor_WithValidNames_ShouldTrimAndBuildFullName()
    {
        var name = new PersonName("  Maria ", " Souza  ");

        name.FirstName.Should().Be("Maria");
        name.LastName.Should().Be("Souza");
        name.FullName.Should().Be("Maria Souza");
        name.ToString().Should().Be("Maria Souza");
    }

    [Theory]
    [InlineData("", "Silva", "O nome é obrigatório")]
    [InlineData("Joao", "", "O sobrenome é obrigatório")]
    public void Constructor_WithEmptyNames_ShouldThrow(string firstName, string lastName, string expectedMessage)
    {
        var act = () => new PersonName(firstName, lastName);

        act.Should().Throw<IdentityDomainException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [InlineData(101, "Silva", "O nome deve conter no máximo 100 caracteres")]
    [InlineData(4, 101, "O sobrenome deve conter no máximo 100 caracteres")]
    public void Constructor_WithTooLongNames_ShouldThrow(object firstLengthOrFirstName, object lastLengthOrLastName, string expectedMessage)
    {
        var firstName = firstLengthOrFirstName is int firstLength
            ? new string('A', firstLength)
            : (string)firstLengthOrFirstName;

        var lastName = lastLengthOrLastName is int lastLength
            ? new string('B', lastLength)
            : (string)lastLengthOrLastName;

        var act = () => new PersonName(firstName, lastName);

        act.Should().Throw<IdentityDomainException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public void Equality_WithSameNames_ShouldBeEqual()
    {
        var left = new PersonName("Ana", "Lima");
        var right = new PersonName("Ana", "Lima");

        left.Should().Be(right);
        left.GetHashCode().Should().Be(right.GetHashCode());
    }
}
