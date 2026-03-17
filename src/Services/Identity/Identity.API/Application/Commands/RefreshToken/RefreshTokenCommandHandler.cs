namespace Identity.API.Application.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    ITokenProvider tokenProvider)
        : ICommandHandler<RefreshTokenCommand, SignInViewModel>
{
    public async Task<SignInViewModel> Handle(
        RefreshTokenCommand command,
        CancellationToken ct)
    {
        var user = await userRepository.GetByRefreshTokenAsync(command.RefreshToken, ct)
            ?? throw new IdentityDomainException("Token de atualização inválido");

        user.RevokeRefreshToken(command.RefreshToken);

        var roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList();
        var accessToken = tokenProvider.GenerateAccessToken(user, roles);
        var newRefreshToken = tokenProvider.GenerateRefreshToken();

        user.GenerateRefreshToken(newRefreshToken, expirationDays: 7);

        userRepository.Update(user);
        await userRepository.UnitOfWork.SaveEntitiesAsync(ct);

        return new SignInViewModel(
            user.Id,
            user.Email.Value,
            user.Name.FullName,
            accessToken.Value,
            newRefreshToken,
            accessToken.ExpiresAt,
            roles);
    }
}
