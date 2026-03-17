namespace Identity.API.Application.Commands.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório")
            .EmailAddress().WithMessage("O e-mail informado é inválido")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A nova senha é obrigatória")
            .MinimumLength(8).WithMessage("A senha deve conter no mínimo 8 caracteres")
            .Matches("[A-Z]").WithMessage("A senha deve conter no mínimo uma letra maiúscula")
            .Matches("[a-z]").WithMessage("A senha deve conter no mínimo uma letra minúscula")
            .Matches("[0-9]").WithMessage("A senha deve conter no mínimo um número")
            .Matches("\\W").WithMessage("A senha deve conter no mínimo um caractere especial");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("O sobrenome é obrigatório")
            .MaximumLength(100);

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("O CPF é obrigatório")
            .MinimumLength(11).WithMessage("CPF inválido")
            .MaximumLength(15).WithMessage("CPF inválido");
    }
}
