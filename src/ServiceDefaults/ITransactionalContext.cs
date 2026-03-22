using Microsoft.EntityFrameworkCore.Storage;

namespace ServiceDefaults;

public interface ITransactionalContext
{
    bool HasActiveTransaction { get; }

    IExecutionStrategy CreateExecutionStrategy();

    Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default);
}
