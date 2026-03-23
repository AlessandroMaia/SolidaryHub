namespace Identity.UnitTests.Domain.ValueObjects;

public sealed class TokenTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldCreateToken()
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(10);

        var token = new Token("access-token", expiresAt);

        token.Value.Should().Be("access-token");
        token.ExpiresAt.Should().Be(expiresAt);
        token.IsExpired.Should().BeFalse();
        ((string)token).Should().Be("access-token");
    }

    [Fact]
    public void Constructor_WithEmptyValue_ShouldThrow()
    {
        var act = () => new Token("", DateTime.UtcNow.AddMinutes(5));

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("O valor do token é obrigatório");
    }

    [Fact]
    public void Constructor_WithExpiredDate_ShouldThrow()
    {
        var act = () => new Token("access-token", DateTime.UtcNow.AddSeconds(-1));

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("A expiração do token deve ser maior que o horário atual");
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(15);
        var left = new Token("token", expiresAt);
        var right = new Token("token", expiresAt);

        left.Should().Be(right);
    }

    [Fact]
    public void ProtectedConstructor_ShouldBeInstantiableForEf()
    {
        Activator.CreateInstance(typeof(Token), nonPublic: true).Should().NotBeNull();
    }
}
