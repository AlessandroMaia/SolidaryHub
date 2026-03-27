using ServiceDefaults.Contracts;

namespace ServiceDefaults.Behaviors;

public sealed class IdempotencyBehavior<TRequest, TResponse>(
    IRequestManager requestManager,
    ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, ICommand<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IIdempotentCommand<TResponse> idempotentRequest ||
            idempotentRequest.RequestId == Guid.Empty)
        {
            return await next(cancellationToken);
        }

        if (await requestManager.ExistAsync(idempotentRequest.RequestId, cancellationToken))
        {
            logger.LogWarning(
                "Requisição duplicada detectada: {RequestId} para {CommandName}",
                idempotentRequest.RequestId,
                typeof(TRequest).Name);

            throw new DuplicateRequestException($"A requisição {idempotentRequest.RequestId} já foi processada.");
        }

        await requestManager.CreateRequestForCommandAsync<TRequest>(idempotentRequest.RequestId, cancellationToken);

        logger.LogInformation(
            "Processando comando idempotente {CommandName} com RequestId {RequestId}",
            typeof(TRequest).Name,
            idempotentRequest.RequestId);

        return await next(cancellationToken);
    }
}

public sealed class IdempotencyBehavior<TRequest>(
    IRequestManager requestManager,
    ILogger<IdempotencyBehavior<TRequest>> logger)
    : IPipelineBehavior<TRequest>
    where TRequest : notnull, ICommand
{
    public async Task Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        if (request is not IIdempotentCommand idempotentRequest ||
            idempotentRequest.RequestId == Guid.Empty)
        {
            await next(cancellationToken);
            return;
        }

        if (await requestManager.ExistAsync(idempotentRequest.RequestId, cancellationToken))
        {
            logger.LogWarning(
                "Requisição duplicada detectada: {RequestId} para {CommandName}",
                idempotentRequest.RequestId,
                typeof(TRequest).Name);

            throw new DuplicateRequestException($"A requisição {idempotentRequest.RequestId} já foi processada.");
        }

        await requestManager.CreateRequestForCommandAsync<TRequest>(idempotentRequest.RequestId, cancellationToken);

        logger.LogInformation(
            "Processando comando idempotente {CommandName} com RequestId {RequestId}",
            typeof(TRequest).Name,
            idempotentRequest.RequestId);

        await next(cancellationToken);
    }
}
