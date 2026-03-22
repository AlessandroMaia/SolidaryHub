namespace Campaign.Domain.AggregatesModel.CampaignAggregate.Entities;

public class Campaign : Entity, IAggregateRoot
{
    private readonly List<CampaignStatusHistory> _statusHistory = [];
    private readonly List<DonationLedgerEntry> _donationLedger = [];

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public CampaignPeriod Period { get; private set; } = null!;
    public DateTime StartDate => Period.StartDate;
    public DateTime EndDate => Period.EndDate;
    public Money FinancialGoal { get; private set; } = null!;
    public decimal FinancialGoalAmount => FinancialGoal.Amount;
    public CampaignStatus Status { get; private set; }
    public int CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public CampaignTotal Total { get; private set; } = null!;

    public IReadOnlyCollection<CampaignStatusHistory> StatusHistory => _statusHistory.AsReadOnly();
    public IReadOnlyCollection<DonationLedgerEntry> DonationLedger => _donationLedger.AsReadOnly();

    protected Campaign() { }

    public static Campaign Create(
        string title,
        string description,
        DateTime startDate,
        DateTime endDate,
        decimal financialGoalAmount,
        int createdByUserId)
    {
        Validate(title, description, startDate, endDate, financialGoalAmount);

        var campaign = new Campaign
        {
            Title = title.Trim(),
            Description = description.Trim(),
            Period = new CampaignPeriod(startDate, endDate),
            FinancialGoal = new Money(financialGoalAmount, "BRL"),
            Status = CampaignStatus.Active,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            Total = CampaignTotal.Create()
        };

        campaign.AddDomainEvent(new CampaignCreatedDomainEvent(
            campaign.Title,
            campaign.StartDate,
            campaign.EndDate,
            campaign.FinancialGoalAmount));

        return campaign;
    }

    public void UpdateDetails(
        string title,
        string description,
        DateTime startDate,
        DateTime endDate,
        decimal financialGoalAmount)
    {
        Validate(title, description, startDate, endDate, financialGoalAmount);

        if (Status is CampaignStatus.Cancelled or CampaignStatus.Completed)
            throw new CampaignDomainException("Não é possível alterar uma campanha encerrada.");

        Title = title.Trim();
        Description = description.Trim();
        Period = new CampaignPeriod(startDate, endDate);
        FinancialGoal = new Money(financialGoalAmount, FinancialGoal.Currency);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CampaignUpdatedDomainEvent(Id, Title, StartDate, EndDate, FinancialGoalAmount));
    }

    public void Cancel(int changedByUserId, string? reason = null)
    {
        if (Status == CampaignStatus.Cancelled)
            return;

        if (Status == CampaignStatus.Completed)
            throw new CampaignDomainException("Não é possível cancelar uma campanha concluída.");

        var oldStatus = Status;
        Status = CampaignStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddStatusHistory(oldStatus, Status, reason ?? "Campanha cancelada", changedByUserId);
        AddDomainEvent(new CampaignCancelledDomainEvent(Id, reason));
    }

    public void Complete(int changedByUserId, string? reason = null)
    {
        if (Status == CampaignStatus.Completed)
            return;

        if (Status == CampaignStatus.Cancelled)
            throw new CampaignDomainException("Não é possível concluir uma campanha cancelada.");

        var oldStatus = Status;
        Status = CampaignStatus.Completed;
        UpdatedAt = DateTime.UtcNow;

        AddStatusHistory(oldStatus, Status, reason ?? "Campanha concluída", changedByUserId);
        AddDomainEvent(new CampaignCompletedDomainEvent(Id, reason));
    }

    public void ApplyDonation(int donationIntentId, int donorUserId, decimal amount, string source, string? correlationId = null)
    {
        if (Status != CampaignStatus.Active)
            throw new CampaignDomainException("A campanha não está ativa para receber doações.");

        if (amount <= 0)
            throw new CampaignDomainException("O valor da doação deve ser maior que zero.");

        if (Period.IsExpired(DateTime.UtcNow))
            throw new CampaignDomainException("A campanha está encerrada.");

        var entry = DonationLedgerEntry.Create(Id, donationIntentId, donorUserId, amount, source, correlationId);

        _donationLedger.Add(entry);
        Total = Total.ApplyDonation(amount, entry.ProcessedAt);

        AddDomainEvent(new DonationIntentAppliedToCampaignDomainEvent(Id, donationIntentId, amount));
        AddDomainEvent(new CampaignTotalUpdatedDomainEvent(Id, Total.TotalAmountRaised, Total.TotalDonationsCount));
    }

    private void AddStatusHistory(CampaignStatus? oldStatus, CampaignStatus newStatus, string reason, int changedByUserId)
    {
        _statusHistory.Add(CampaignStatusHistory.Create(Id, oldStatus, newStatus, reason, changedByUserId));
    }

    private static void Validate(
        string title,
        string description,
        DateTime startDate,
        DateTime endDate,
        decimal financialGoalAmount)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new CampaignDomainException("O título da campanha é obrigatório.");

        if (string.IsNullOrWhiteSpace(description))
            throw new CampaignDomainException("A descrição da campanha é obrigatória.");

        if (financialGoalAmount <= 0)
            throw new CampaignDomainException("A meta financeira deve ser maior que zero.");

        _ = new CampaignPeriod(startDate, endDate);
    }
}
