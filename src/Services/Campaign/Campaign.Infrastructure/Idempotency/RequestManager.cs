using Npgsql;

namespace Campaign.Infrastructure.Idempotency;

public class RequestManager(CampaignContext context) : IRequestManager
{
    private readonly CampaignContext _context = context 
        ?? throw new ArgumentNullException(nameof(context));

    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.ClientRequests.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task CreateRequestForCommandAsync<T>(Guid id, CancellationToken cancellationToken = default)
    {
        _context.Add(new ClientRequest
        {
            Id = id,
            Name = typeof(T).Name,
            Time = DateTime.UtcNow
        });

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            throw new CampaignDomainException($"A requisição {id} já foi processada.", ex);
        }
    }
}
