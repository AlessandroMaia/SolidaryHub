namespace Campaign.Domain.AggregatesModel.DonationIntentAggregate.Entities;

public class DonationIntent : Entity, IAggregateRoot
{
    private readonly List<DonationIntentProcessingLog> _processingLogs = [];
    private readonly List<DonationIntentDeadLetter> _deadLetters = [];

    public int CampaignId { get; private set; }
    public int DonorUserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;
    public DonationIntentStatus Status { get; private set; }
    public string Source { get; private set; } = null!;
    public string? CorrelationId { get; private set; }
    public string? MessageId { get; private set; }

    public DateTime RequestedAt { get; private set; }
    public DateTime? ValidatedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public string? RejectionReason { get; private set; }

    public IReadOnlyCollection<DonationIntentProcessingLog> ProcessingLogs => _processingLogs.AsReadOnly();
    public IReadOnlyCollection<DonationIntentDeadLetter> DeadLetters => _deadLetters.AsReadOnly();

    protected DonationIntent() { }

    public static DonationIntent Create(
        int campaignId,
        int donorUserId,
        decimal amount,
        string currency,
        string source,
        string? correlationId = null,
        string? messageId = null)
    {
        if (campaignId <= 0)
            throw new CampaignDomainException("A campanha informada é inválida.");

        if (donorUserId <= 0)
            throw new CampaignDomainException("O doador informado é inválido.");

        if (amount <= 0)
            throw new CampaignDomainException("O valor da doação deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new CampaignDomainException("A moeda da doação é obrigatória.");

        if (string.IsNullOrWhiteSpace(source))
            throw new CampaignDomainException("A origem da intenção de doação é obrigatória.");

        var donationIntent = new DonationIntent
        {
            CampaignId = campaignId,
            DonorUserId = donorUserId,
            Amount = amount,
            Currency = currency.Trim().ToUpperInvariant(),
            Source = source.Trim(),
            CorrelationId = correlationId,
            MessageId = messageId,
            Status = DonationIntentStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        donationIntent.AddDomainEvent(new DonationIntentCreatedDomainEvent(
            donationIntent.Id,
            donationIntent.CampaignId,
            donationIntent.DonorUserId,
            donationIntent.Amount));

        return donationIntent;
    }

    public void ValidateForProcessing()
    {
        if (Status != DonationIntentStatus.Pending)
            throw new CampaignDomainException("A intenção de doação não está pendente para validação.");

        Status = DonationIntentStatus.Validated;
        ValidatedAt = DateTime.UtcNow;

        AddDomainEvent(new DonationIntentValidatedDomainEvent(Id, CampaignId, Amount));
    }

    public void MarkAsPublished()
    {
        if (Status != DonationIntentStatus.Validated)
            throw new CampaignDomainException("A intenção de doação precisa estar validada para ser publicada.");

        Status = DonationIntentStatus.Published;
    }

    public void MarkAsProcessing(string workerName)
    {
        if (Status is not DonationIntentStatus.Published and not DonationIntentStatus.Failed)
            throw new CampaignDomainException("A intenção de doação não está disponível para processamento.");

        Status = DonationIntentStatus.Processing;
        AddProcessingLog(workerName, WorkerProcessingStatus.Processing, null);
    }

    public void MarkAsProcessed(string workerName)
    {
        if (Status != DonationIntentStatus.Processing)
            throw new CampaignDomainException("A intenção de doação não está em processamento.");

        Status = DonationIntentStatus.Processed;
        ProcessedAt = DateTime.UtcNow;

        AddProcessingLog(workerName, WorkerProcessingStatus.Processed, null);

        AddDomainEvent(new DonationIntentProcessedDomainEvent(
            Id,
            CampaignId,
            DonorUserId,
            Amount,
            ProcessedAt.Value));
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new CampaignDomainException("O motivo da rejeição é obrigatório.");

        if (Status == DonationIntentStatus.Processed)
            throw new CampaignDomainException("Não é possível rejeitar uma intenção de doação já processada.");

        Status = DonationIntentStatus.Rejected;
        RejectedAt = DateTime.UtcNow;
        RejectionReason = reason.Trim();

        AddDomainEvent(new DonationIntentRejectedDomainEvent(Id, CampaignId, RejectionReason));
    }

    public void MarkAsFailed(string workerName, string errorMessage, bool sendToDeadLetter = false)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new CampaignDomainException("A mensagem de erro é obrigatória.");

        Status = DonationIntentStatus.Failed;

        AddProcessingLog(workerName, WorkerProcessingStatus.Failed, errorMessage);

        if (sendToDeadLetter)
            AddDeadLetter(errorMessage);

        AddDomainEvent(new DonationIntentFailedDomainEvent(Id, CampaignId, errorMessage));
    }

    private void AddProcessingLog(string workerName, WorkerProcessingStatus status, string? errorMessage)
    {
        _processingLogs.Add(DonationIntentProcessingLog.Create(Id, CampaignId, workerName, status, errorMessage));
    }

    private void AddDeadLetter(string errorMessage)
    {
        _deadLetters.Add(DonationIntentDeadLetter.Create(Id, MessageId, CampaignId, errorMessage));
    }
}
