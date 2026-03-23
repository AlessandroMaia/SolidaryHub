using Identity.UnitTests.Helpers;

namespace Identity.UnitTests.Application.Commands;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        _handler = new RegisterUserCommandHandler(_userRepository, _passwordHasher, NullLogger<RegisterUserCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateUserAndAssignDefaultRole()
    {
        var role = UserTestFactory.CreateRole(Roles.Donor, 2);
        _userRepository.ExistsByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.GetRoleByNameAsync(Roles.Donor, Arg.Any<CancellationToken>()).Returns(role);
        _passwordHasher.Hash("StrongP@ss1").Returns("HASH");

        var result = await _handler.Handle(
            new RegisterUserCommand("user@example.com", "StrongP@ss1", "Joao", "Silva", "52998224725"),
            CancellationToken.None);

        result.Email.Should().Be("user@example.com");
        result.FullName.Should().Be("Joao Silva");
        _userRepository.Received(1).Add(Arg.Is<User>(u => u.Email.Value == "user@example.com" && u.UserRoles.Count == 1));
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrow()
    {
        _userRepository.ExistsByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(true);

        var act = async () => await _handler.Handle(
            new RegisterUserCommand("user@example.com", "StrongP@ss1", "Joao", "Silva", "52998224725"),
            CancellationToken.None);

        await act.Should().ThrowAsync<IdentityDomainException>()
            .WithMessage("Email inválido ou não disponível");
    }

    [Fact]
    public async Task Handle_WhenDefaultRoleDoesNotExist_ShouldCreateUserWithoutRole()
    {
        _userRepository.ExistsByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.GetRoleByNameAsync(Roles.Donor, Arg.Any<CancellationToken>()).Returns((Role?)null);
        _passwordHasher.Hash("StrongP@ss1").Returns("HASH");

        await _handler.Handle(
            new RegisterUserCommand("user@example.com", "StrongP@ss1", "Joao", "Silva", "52998224725"),
            CancellationToken.None);

        _userRepository.Received(1).Add(Arg.Is<User>(u => !u.UserRoles.Any()));
    }
}
