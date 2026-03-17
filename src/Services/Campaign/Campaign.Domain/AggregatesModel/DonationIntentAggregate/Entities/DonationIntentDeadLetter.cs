namespace Campaign.Domain.AggregatesModel.DonationIntentAggregate.Entities;

public class DonationIntentDeadLetter : Entity
{
    public int DonationIntentId { get; private set; }
    public string? MessageId { get; private set; }
    public int CampaignId { get; private set; }
    public string ErrorMessage { get; private set; } = null!;
    public DateTime FailedAt { get; private set; }

    protected DonationIntentDeadLetter() { }

    private DonationIntentDeadLetter(
        int donationIntentId,
        string? messageId,
        int campaignId,
        string errorMessage)
    {
        DonationIntentId = donationIntentId;
        MessageId = messageId;
        CampaignId = campaignId;
        ErrorMessage = errorMessage;
        FailedAt = DateTime.UtcNow;
    }

    public static DonationIntentDeadLetter Create(
        int donationIntentId,
        string? messageId,
        int campaignId,
        string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new CampaignDomainException("A mensagem de erro é obrigatória.");

        return new DonationIntentDeadLetter(
            donationIntentId,
            messageId,
            campaignId,
            errorMessage.Trim());
    }
}
