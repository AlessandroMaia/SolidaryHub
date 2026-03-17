using System.Data;

namespace Campaign.Infrastructure;

/// <remark>
/// dotnet ef migrations add --startup-project ..\Campaign.API --context CampaignContext [migration-name]
/// </remark>
public class CampaignContext(
    DbContextOptions<CampaignContext> options,
    IMediator? mediator = null) 
        : DbContext(options), IUnitOfWork
{
    public DbSet<CampaignEntity> Campaigns { get; set; } = null!;
    public DbSet<CampaignStatusHistory> CampaignStatusHistories { get; set; } = null!;
    public DbSet<CampaignTotal> CampaignTotals { get; set; } = null!;
    public DbSet<DonationLedgerEntry> DonationLedgerEntries { get; set; } = null!;
    public DbSet<DonationIntent> DonationIntents { get; set; } = null!;
    public DbSet<DonationIntentDeadLetter> DonationIntentDeadLetters { get; set; } = null!;
    public DbSet<DonationIntentProcessingLog> DonationIntentProcessingLogs { get; set; } = null!;

    private readonly IMediator? _mediator = mediator;
    private IDbContextTransaction? _currentTransaction;

    public IDbContextTransaction? GetCurrentTransaction () => _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction is not null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("campaign_db");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CampaignContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        if (_mediator is not null)
            await _mediator.DispatchDomainEventsAsync(this);

        _ = await base.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction != _currentTransaction) 
            throw new InvalidOperationException($"Transação #{transaction.TransactionId} não é a atual");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
