namespace Identity.UnitTests.Domain.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Constructor_WithValidEmail_ShouldNormalizeValue()
    {
        var email = new Email("  USER@Example.COM ");

        email.Value.Should().Be("user@example.com");
        email.ToString().Should().Be("user@example.com");
    }

    [Fact]
    public void Constructor_WithEmptyValue_ShouldThrow()
    {
        var act = () => new Email("");

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("O e-mail é obrigatório");
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ShouldThrow()
    {
        var act = () => new Email("invalido");

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("O e-mail informado é inválido");
    }

    [Fact]
    public void Equality_WithSameNormalizedValue_ShouldBeEqual()
    {
        var left = new Email("user@example.com");
        var right = new Email("USER@example.com");

        left.Should().Be(right);
        (left == right).Should().BeTrue();
        left.GetHashCode().Should().Be(right.GetHashCode());
    }
}
