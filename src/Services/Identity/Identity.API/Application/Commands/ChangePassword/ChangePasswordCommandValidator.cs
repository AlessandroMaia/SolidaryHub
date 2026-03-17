namespace Identity.API.Application.Commands.ChangePassword;

internal sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("A senha atual é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("A nova senha é obrigatória")
            .MinimumLength(8).WithMessage("A senha deve conter no mínimo 8 caracteres")
            .Matches("[A-Z]").WithMessage("A senha deve conter no mínimo uma letra maiúscula")
            .Matches("[a-z]").WithMessage("A senha deve conter no mínimo uma letra minúscula")
            .Matches("[0-9]").WithMessage("A senha deve conter no mínimo um número")
            .Matches("\\W").WithMessage("A senha deve conter no mínimo um caractere especial");
    }
}
