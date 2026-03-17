namespace Identity.API.Application.Commands.SignIn;

public sealed record SignInCommand(string Email, string Password) 
    : ICommand<SignInViewModel>;