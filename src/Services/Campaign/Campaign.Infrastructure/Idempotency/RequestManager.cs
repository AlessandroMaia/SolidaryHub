namespace Campaign.Infrastructure.Idempotency;

public class RequestManager(CampaignContext context) : IRequestManager
{
    private readonly CampaignContext _context = context 
        ?? throw new ArgumentNullException(nameof(context));

    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.ClientRequests.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task CreateRequestForCommandAsync<T>(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await ExistAsync(id, cancellationToken);

        var request = exists ?
            throw new CampaignDomainException($"Requisição com o ID {id} já existe") :
            new ClientRequest()
            {
                Id = id,
                Name = typeof(T).Name,
                Time = DateTime.UtcNow
            };

        _context.Add(request);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
