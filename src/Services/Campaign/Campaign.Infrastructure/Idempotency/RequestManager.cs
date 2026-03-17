namespace Campaign.Infrastructure.Idempotency;

public class RequestManager(CampaignContext context) : IRequestManager
{
    private readonly CampaignContext _context = context 
        ?? throw new ArgumentNullException(nameof(context));

    public async Task<bool> ExistAsync(Guid id)
    {
        var request = await _context.
            FindAsync<ClientRequest>(id);

        return request != null;
    }

    public async Task CreateRequestForCommandAsync<T>(Guid id)
    {
        var exists = await ExistAsync(id);

        var request = exists ?
            throw new CampaignDomainException($"Requisição com o ID {id} já existe") :
            new ClientRequest()
            {
                Id = id,
                Name = typeof(T).Name,
                Time = DateTime.UtcNow
            };

        _context.Add(request);

        await _context.SaveChangesAsync();
    }
}