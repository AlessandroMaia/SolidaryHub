namespace Campaign.Domain.Exceptions;

public class CampaignDomainException : Exception
{
    public CampaignDomainException() { }

    public CampaignDomainException(string message)
        : base(message) { }

    public CampaignDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}