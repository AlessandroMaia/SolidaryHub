namespace Identity.API.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Cpf) : ICommand<RegisterUserViewModel>;