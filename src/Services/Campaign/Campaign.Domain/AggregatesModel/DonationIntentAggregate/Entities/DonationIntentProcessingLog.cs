namespace Campaign.Domain.AggregatesModel.DonationIntentAggregate.Entities;

public class DonationIntentProcessingLog : Entity
{
    public int DonationIntentId { get; private set; }
    public int CampaignId { get; private set; }
    public string WorkerName { get; private set; } = null!;
    public WorkerProcessingStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastAttemptAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    protected DonationIntentProcessingLog() { }

    private DonationIntentProcessingLog(
        int donationIntentId,
        int campaignId,
        string workerName,
        WorkerProcessingStatus status,
        string? errorMessage)
    {
        DonationIntentId = donationIntentId;
        CampaignId = campaignId;
        WorkerName = workerName;
        Status = status;
        ErrorMessage = errorMessage;
        AttemptCount = 1;
        CreatedAt = DateTime.UtcNow;
        LastAttemptAt = DateTime.UtcNow;
    }

    public static DonationIntentProcessingLog Create(
        int donationIntentId,
        int campaignId,
        string workerName,
        WorkerProcessingStatus status,
        string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(workerName))
            throw new CampaignDomainException("O nome do worker é obrigatório.");

        return new DonationIntentProcessingLog(
            donationIntentId,
            campaignId,
            workerName.Trim(),
            status,
            errorMessage);
    }
}
