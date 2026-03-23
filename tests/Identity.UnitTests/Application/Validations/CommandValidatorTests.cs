namespace Identity.UnitTests.Application.Validations;

public sealed class CommandValidatorTests
{
    [Fact]
    public void RegisterUserValidator_WithValidCommand_ShouldPass()
    {
        var validator = new RegisterUserCommandValidator();
        var result = validator.Validate(new RegisterUserCommand("user@example.com", "StrongP@ss1", "Joao", "Silva", "52998224725"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RegisterUserValidator_WithInvalidCommand_ShouldReturnErrors()
    {
        var validator = new RegisterUserCommandValidator();
        var result = validator.Validate(new RegisterUserCommand("", "weak", "", "", ""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
        result.Errors.Should().Contain(e => e.PropertyName == "Cpf");
    }

    [Fact]
    public void SignInValidator_WithInvalidCommand_ShouldReturnErrors()
    {
        var validator = new SignInCommandValidator();
        var result = validator.Validate(new SignInCommand("", ""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void ChangePasswordValidator_WithInvalidCommand_ShouldReturnErrors()
    {
        var validator = new ChangePasswordCommandValidator();
        var result = validator.Validate(new ChangePasswordCommand(1, "", "weak"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CurrentPassword");
        result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
    }
}
