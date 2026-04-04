using Identity.UnitTests.Helpers;

namespace Identity.UnitTests.Application.Commands;

public sealed class RefreshTokenCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenProvider _tokenProvider = Substitute.For<ITokenProvider>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _userRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
        _handler = new RefreshTokenCommandHandler(_userRepository, _tokenProvider);
    }

    [Fact]
    public async Task Handle_WithValidRefreshToken_ShouldRotateTokens()
    {
        var user = UserTestFactory.CreateActiveUser(id: 1);
        var role = UserTestFactory.CreateRole(Roles.Manager, 2);
        UserTestFactory.AssignRole(user, role);
        var oldToken = user.GenerateRefreshToken("old-token");

        _userRepository.GetByRefreshTokenAsync("old-token", Arg.Any<CancellationToken>()).Returns(user);
        _tokenProvider.GenerateAccessToken(user, Arg.Any<IEnumerable<string>>()).Returns(new Token("new-access", DateTime.UtcNow.AddMinutes(10)));
        _tokenProvider.GenerateRefreshToken().Returns("new-refresh");

        var result = await _handler.Handle(new RefreshTokenCommand("old-token"), CancellationToken.None);

        oldToken.IsRevoked.Should().BeTrue();
        user.RefreshTokens.Should().Contain(rt => rt.Matches("new-refresh"));
        result.AccessToken.Should().Be("new-access");
        result.RefreshToken.Should().Be("new-refresh");
        result.Roles.Should().ContainSingle(Roles.Manager);
        _userRepository.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidRefreshToken_ShouldThrow()
    {
        _userRepository.GetByRefreshTokenAsync("invalid", Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _handler.Handle(new RefreshTokenCommand("invalid"), CancellationToken.None);

        await act.Should().ThrowAsync<IdentityDomainException>()
            .WithMessage("Token de atualização inválido");
    }
}
