namespace Identity.API.Application.Commands.SignIn;

internal sealed class SignInCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    ILogger<SignInCommandHandler> logger)
        : ICommandHandler<SignInCommand, SignInViewModel>
{
    public async Task<SignInViewModel> Handle(
        SignInCommand command,
        CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(command.Email, ct)
            ?? throw new IdentityDomainException("Email ou senha inválidos");

        if (user.Status != UserStatus.Active)
            throw new IdentityDomainException("Usuário não está ativo");

        if (!passwordHasher.Verify(command.Password, user.PasswordHash.Hash))
            throw new IdentityDomainException("Email ou senha inválidos");

        var roles = user.UserRoles
            .Select(ur => ur.Role!.Name)
            .ToList();

        var accessToken = tokenProvider.GenerateAccessToken(user, roles);
        var refreshTokenValue = tokenProvider.GenerateRefreshToken();

        user.GenerateRefreshToken(refreshTokenValue, expirationDays: 7);
        user.RecordLogin();

        userRepository.Update(user);
        await userRepository.UnitOfWork.SaveEntitiesAsync(ct);

        logger.LogInformation("Usuário autenticado com o e-mail {Email}", user.Email.Value);

        return new SignInViewModel(
            user.Id,
            user.Email.Value,
            user.Name.FullName,
            accessToken.Value,
            refreshTokenValue,
            accessToken.ExpiresAt,
            roles);
    }
}
