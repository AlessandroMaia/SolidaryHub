namespace ServiceDefaults;

public interface ITransactionEventPublisher
{
    Task PublishAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
