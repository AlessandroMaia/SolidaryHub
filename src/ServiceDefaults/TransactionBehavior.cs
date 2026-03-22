using Mediator.Behaviors;
using Mediator.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ServiceDefaults;

public sealed class TransactionBehavior<TCommand, TResponse>(
    ITransactionalContext context,
    IEnumerable<ITransactionEventPublisher> transactionEventPublishers,
    ILogger<TransactionBehavior<TCommand, TResponse>> logger)
    : IPipelineBehavior<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(
        TCommand command,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = default(TResponse);
        var typeName = command.GetType().Name;

        try
        {
            if (context.HasActiveTransaction)
            {
                logger.LogDebug("Já existe uma transação ativa para {CommandName}; o comportamento transacional será ignorado", typeName);
                return await next(ct);
            }

            var strategy = context.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                await using var transaction = await context.BeginTransactionAsync(ct);

                if (transaction is null)
                {
                    response = await next(ct);
                    return;
                }

                using (logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                {
                    logger.LogInformation("Iniciando transação {TransactionId} para {CommandName} ({@Command})",
                        transaction.TransactionId, typeName, command);

                    response = await next(ct);

                    logger.LogInformation("Confirmando transação {TransactionId} para {CommandName}",
                        transaction.TransactionId, typeName);

                    await context.CommitTransactionAsync(transaction, ct);

                    transactionId = transaction.TransactionId;
                }

                foreach (var transactionEventPublisher in transactionEventPublishers)
                    await transactionEventPublisher.PublishAsync(transactionId, ct);
            });

            return response!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar a transação para {CommandName} ({@Command})", typeName, command);

            throw;
        }
    }
}

public sealed class TransactionBehavior<TCommand>(
    ITransactionalContext context,
    IEnumerable<ITransactionEventPublisher> transactionEventPublishers,
    ILogger<TransactionBehavior<TCommand>> logger)
    : IPipelineBehavior<TCommand>
        where TCommand : ICommand
{
    public async Task Handle(
        TCommand command,
        RequestHandlerDelegate next,
        CancellationToken ct)
    {
        var typeName = command.GetType().Name;

        try
        {
            if (context.HasActiveTransaction)
            {
                logger.LogDebug("Já existe uma transação ativa para {CommandName}; o comportamento transacional será ignorado", typeName);
                await next(ct);
                return;
            }

            var strategy = context.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                await using var transaction = await context.BeginTransactionAsync(ct);

                if (transaction is null)
                {
                    await next(ct);
                    return;
                }

                using (logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                {
                    logger.LogInformation("Iniciando transação {TransactionId} para {CommandName} ({@Command})",
                        transaction.TransactionId, typeName, command);

                    await next(ct);

                    logger.LogInformation("Confirmando transação {TransactionId} para {CommandName}",
                        transaction.TransactionId, typeName);

                    await context.CommitTransactionAsync(transaction, ct);

                    transactionId = transaction.TransactionId;
                }

                foreach (var transactionEventPublisher in transactionEventPublishers)
                    await transactionEventPublisher.PublishAsync(transactionId, ct);
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar a transação para {CommandName} ({@Command})", typeName, command);

            throw;
        }
    }
}
