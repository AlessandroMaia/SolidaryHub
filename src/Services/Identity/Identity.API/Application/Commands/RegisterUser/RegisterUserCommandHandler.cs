namespace Identity.API.Application.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ILogger<RegisterUserCommandHandler> logger)
        : ICommandHandler<RegisterUserCommand, RegisterUserViewModel>
{
    public async Task<RegisterUserViewModel> Handle(
        RegisterUserCommand command,
        CancellationToken ct)
    {
        if (await userRepository.ExistsByEmailAsync(command.Email, ct))
            throw new IdentityDomainException("Email inválido ou não disponível");

        Password.ValidateStrength(command.Password);

        var email = new Email(command.Email);
        var passwordHash = Password.FromHash(passwordHasher.Hash(command.Password));
        var name = new PersonName(command.FirstName, command.LastName);
        var cpf = new Cpf(command.Cpf);

        var user = User.Create(email, passwordHash, name, cpf);

        var defaultRole = await userRepository.GetRoleByNameAsync(Roles.Donor, ct);
        if (defaultRole is not null)
            user.AssignRole(defaultRole);

        userRepository.Add(user);
        await userRepository.UnitOfWork.SaveEntitiesAsync(ct);

        logger.LogInformation("Usuário registrado: {Email} ({UserId})",
            user.Email.Value, user.Id);

        return new RegisterUserViewModel(user.Id, user.Email.Value, user.Name.FullName);
    }
}