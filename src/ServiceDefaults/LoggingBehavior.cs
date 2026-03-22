using Mediator.Behaviors;
using Microsoft.Extensions.Logging;

namespace ServiceDefaults;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("[INÍCIO] Processando {RequestName}", requestName);

        try
        {
            var response = await next(cancellationToken);

            logger.LogInformation("[FIM] {RequestName} processado com sucesso", requestName);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ERRO] Falha ao processar {RequestName}", requestName);
            throw;
        }
    }
}

public class LoggingBehavior<TRequest>(ILogger<LoggingBehavior<TRequest>> logger)
    : IPipelineBehavior<TRequest>
    where TRequest : notnull
{
    public async Task Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("[INÍCIO] Processando {RequestName}", requestName);

        try
        {
            await next(cancellationToken);

            logger.LogInformation("[FIM] {RequestName} processado com sucesso", requestName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ERRO] Falha ao processar {RequestName}", requestName);
            throw;
        }
    }
}
