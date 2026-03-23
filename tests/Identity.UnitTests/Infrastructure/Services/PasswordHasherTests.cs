namespace Identity.UnitTests.Infrastructure.Services;

public sealed class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();

    [Fact]
    public void Hash_ShouldGenerateHashWithSalt()
    {
        var hash = _passwordHasher.Hash("StrongP@ss1");

        hash.Should().Contain("-");
        hash.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Verify_WithMatchingPassword_ShouldReturnTrue()
    {
        var hash = _passwordHasher.Hash("StrongP@ss1");

        var result = _passwordHasher.Verify("StrongP@ss1", hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_WithDifferentPassword_ShouldReturnFalse()
    {
        var hash = _passwordHasher.Hash("StrongP@ss1");

        var result = _passwordHasher.Verify("WrongP@ss1", hash);

        result.Should().BeFalse();
    }
}
