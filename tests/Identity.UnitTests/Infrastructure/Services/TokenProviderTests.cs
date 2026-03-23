using Identity.UnitTests.Helpers;

namespace Identity.UnitTests.Infrastructure.Services;

public sealed class TokenProviderTests
{
    private readonly TokenSettings _settings = new()
    {
        Secret = "12345678901234567890123456789012",
        Issuer = "solidarityhub",
        Audience = "solidarityhub-users",
        AccessTokenExpirationMinutes = 15
    };

    [Fact]
    public void GenerateAccessToken_ShouldCreateValidToken()
    {
        var provider = new TokenProvider(Options.Create(_settings));
        var user = UserTestFactory.CreateActiveUser(id: 42);

        var token = provider.GenerateAccessToken(user, [Roles.Donor]);

        token.Value.Should().NotBeNullOrWhiteSpace();
        token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        var principal = provider.ValidateToken(token.Value);
        principal.Should().NotBeNull();
        principal!.Claims.Select(c => c.Value).Should().Contain("42");
        principal.FindAll(ClaimTypes.Role).Select(c => c.Value).Should().Contain(Roles.Donor);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldCreateRandomToken()
    {
        var provider = new TokenProvider(Options.Create(_settings));

        var first = provider.GenerateRefreshToken();
        var second = provider.GenerateRefreshToken();

        first.Should().NotBe(second);
        first.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        var provider = new TokenProvider(Options.Create(_settings));

        var principal = provider.ValidateToken("token-invalido");

        principal.Should().BeNull();
    }
}
