namespace Campaign.Infrastructure.Idempotency;

public interface IRequestManager
{
    Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default);

    Task CreateRequestForCommandAsync<T>(Guid id, CancellationToken cancellationToken = default);
}
