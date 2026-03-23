using Identity.UnitTests.Helpers;

namespace Identity.UnitTests.Application.Commands;

public sealed class ChangePasswordCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _userRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        _handler = new ChangePasswordCommandHandler(_userRepository, _passwordHasher);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldChangePasswordAndRevokeRefreshTokens()
    {
        var user = UserTestFactory.CreateActiveUser(id: 1, passwordHash: "OLD_HASH");
        var refreshToken = user.GenerateRefreshToken("refresh-token");

        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("CurrentP@ss1", "OLD_HASH").Returns(true);
        _passwordHasher.Hash("NewP@ss1").Returns("NEW_HASH");

        await _handler.Handle(new ChangePasswordCommand(1, "CurrentP@ss1", "NewP@ss1"), CancellationToken.None);

        user.PasswordHash.Hash.Should().Be("NEW_HASH");
        refreshToken.IsRevoked.Should().BeTrue();
        _userRepository.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrow()
    {
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _handler.Handle(new ChangePasswordCommand(1, "CurrentP@ss1", "NewP@ss1"), CancellationToken.None);

        await act.Should().ThrowAsync<IdentityDomainException>()
            .WithMessage("Usuário não encontrado");
    }

    [Fact]
    public async Task Handle_WhenCurrentPasswordIsInvalid_ShouldThrow()
    {
        var user = UserTestFactory.CreateActiveUser(id: 1, passwordHash: "OLD_HASH");
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("WrongP@ss1", "OLD_HASH").Returns(false);

        var act = async () => await _handler.Handle(new ChangePasswordCommand(1, "WrongP@ss1", "NewP@ss1"), CancellationToken.None);

        await act.Should().ThrowAsync<IdentityDomainException>()
            .WithMessage("Senha inválida");
    }
}
