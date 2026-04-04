using Identity.UnitTests.Helpers;

namespace Identity.UnitTests.Application.Commands;

public sealed class SignInCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenProvider _tokenProvider = Substitute.For<ITokenProvider>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly SignInCommandHandler _handler;

    public SignInCommandHandlerTests()
    {
        _userRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        _handler = new SignInCommandHandler(
            _userRepository,
            _passwordHasher,
            _tokenProvider,
            NullLogger<SignInCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnTokensAndPersistUser()
    {
        var user = UserTestFactory.CreateActiveUser(id: 1);
        var role = UserTestFactory.CreateRole(Roles.Donor, 2);
        UserTestFactory.AssignRole(user, role);

        _userRepository.GetByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("StrongP@ss1", user.PasswordHash.Hash).Returns(true);
        _tokenProvider.GenerateAccessToken(user, Arg.Any<IEnumerable<string>>()).Returns(new Token("access", DateTime.UtcNow.AddMinutes(10)));
        _tokenProvider.GenerateRefreshToken().Returns("refresh");

        var result = await _handler.Handle(new SignInCommand("user@example.com", "StrongP@ss1"), CancellationToken.None);

        result.AccessToken.Should().Be("access");
        result.RefreshToken.Should().Be("refresh");
        result.Roles.Should().ContainSingle(Roles.Donor);
        user.RefreshTokens.Should().ContainSingle(rt => rt.Matches("refresh"));
        user.LastLoginAt.Should().NotBeNull();
        _userRepository.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrow()
    {
        _userRepository.GetByEmailAsync("missing@example.com", Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _handler.Handle(new SignInCommand("missing@example.com", "StrongP@ss1"), CancellationToken.None);

        await act.Should().ThrowAsync<IdentityDomainException>()
            .WithMessage("Email ou senha inválidos");
    }

    [Fact]
    public async Task Handle_WhenUserIsInactive_ShouldThrow()
    {
        var user = UserTestFactory.CreateActiveUser();
        user.Deactivate();
        _userRepository.GetByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(user);

        var act = async () => await _handler.Handle(new SignInCommand("user@example.com", "StrongP@ss1"), CancellationToken.None);

        await act.Should().ThrowAsync<IdentityDomainException>()
            .WithMessage("Usuário não está ativo");
    }

    [Fact]
    public async Task Handle_WhenPasswordIsInvalid_ShouldThrow()
    {
        var user = UserTestFactory.CreateActiveUser();
        _userRepository.GetByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("wrong", user.PasswordHash.Hash).Returns(false);

        var act = async () => await _handler.Handle(new SignInCommand("user@example.com", "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<IdentityDomainException>()
            .WithMessage("Email ou senha inválidos");
    }
}
