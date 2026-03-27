using SharedKernel.Domain;

namespace SharedKernel.Idempotency;

public sealed class DuplicateRequestException : DomainException
{
    public DuplicateRequestException(string message)
        : base(message) { }

    public DuplicateRequestException(string message, Exception innerException)
        : base(message, innerException) { }
}
