namespace Identity.Domain.Exceptions;

public class IdentityDomainException : DomainException
{
    public IdentityDomainException() { }

    public IdentityDomainException(string message)
        : base(message) { }

    public IdentityDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}