namespace Identity.API.Application.Commands.SignIn;

internal sealed class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório")
            .EmailAddress().WithMessage("O e-mail informado é inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória");
    }
}
