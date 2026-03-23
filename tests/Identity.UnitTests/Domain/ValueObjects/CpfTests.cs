namespace Identity.UnitTests.Domain.ValueObjects;

public sealed class CpfTests
{
    [Theory]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("52998224725", "52998224725")]
    public void Constructor_WithValidCpf_ShouldNormalizeValue(string input, string expected)
    {
        var cpf = new Cpf(input);

        cpf.Value.Should().Be(expected);
        cpf.ToString().Should().Be(expected);
    }

    [Fact]
    public void Constructor_WithEmptyCpf_ShouldThrow()
    {
        var act = () => new Cpf("");

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("CPF é obrigatório");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("529-982-247-25")]
    public void Constructor_WithInvalidFormat_ShouldThrow(string input)
    {
        var act = () => new Cpf(input);

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("Formato do CPF inválido");
    }

    [Theory]
    [InlineData("11111111111")]
    [InlineData("52998224724")]
    public void Constructor_WithInvalidDigits_ShouldThrow(string input)
    {
        var act = () => new Cpf(input);

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("CPF inválido");
    }

    [Fact]
    public void Equality_WithSameNormalizedValue_ShouldBeEqual()
    {
        var left = new Cpf("529.982.247-25");
        var right = new Cpf("52998224725");

        left.Should().Be(right);
    }
}
