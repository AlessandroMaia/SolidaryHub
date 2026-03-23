namespace Identity.UnitTests.Domain.ValueObjects;

public sealed class PasswordTests
{
    [Fact]
    public void FromHash_WithValidHash_ShouldCreatePassword()
    {
        var password = Password.FromHash("HASH");

        password.Hash.Should().Be("HASH");
        password.ToString().Should().Be("********");
    }

    [Fact]
    public void FromHash_WithEmptyHash_ShouldThrow()
    {
        var act = () => Password.FromHash("");

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("O hash de senha é obrigatório");
    }

    [Fact]
    public void ValidateStrength_WithValidPassword_ShouldNotThrow()
    {
        var act = () => Password.ValidateStrength("StrongP@ss1");

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("", "A senha é obrigatória")]
    [InlineData("Aa1!", "A senha deve conter no mínimo 8 caracteres")]
    [InlineData("strongp@ss1", "A senha deve conter no mínimo uma letra maiúscula")]
    [InlineData("STRONGP@SS1", "A senha deve conter no mínimo uma letra minúscula")]
    [InlineData("StrongP@ss", "A senha deve conter no mínimo um número")]
    [InlineData("StrongPass1", "A senha deve conter no mínimo um caractere especial")]
    public void ValidateStrength_WithInvalidPassword_ShouldThrow(string password, string expectedMessage)
    {
        var act = () => Password.ValidateStrength(password);

        act.Should().Throw<IdentityDomainException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public void ProtectedConstructor_ShouldBeInstantiableForEf()
    {
        Activator.CreateInstance(typeof(Password), nonPublic: true).Should().NotBeNull();
    }
}
