namespace SharedKernel.Results;

public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors)
        : base("General.Validation", "Um ou mais erros de validação lançados", ErrorType.Validation)
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new([.. results.Where(r => r.IsFailure).Select(r => r.Error)]);
}
