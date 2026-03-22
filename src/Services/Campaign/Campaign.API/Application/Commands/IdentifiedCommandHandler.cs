using Campaign.Infrastructure.Idempotency;

namespace Campaign.API.Application.Commands;

internal sealed class IdentifiedCommandHandler<TCommand, TResponse>(
    IMediator mediator,
    IRequestManager requestManager,
    ILogger<IdentifiedCommandHandler<TCommand, TResponse>> logger)
    : ICommandHandler<IdentifiedCommand<TCommand, TResponse>, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(IdentifiedCommand<TCommand, TResponse> command, CancellationToken ct)
    {
        if (await requestManager.ExistAsync(command.Id, ct))
        {
            logger.LogWarning("Requisição duplicada detectada: {RequestId}", command.Id);
            throw new CampaignDomainException($"A requisição {command.Id} já foi processada.");
        }

        await requestManager.CreateRequestForCommandAsync<TCommand>(command.Id, ct);
        return await mediator.Send(command.Command, ct);
    }
}

internal sealed class IdentifiedCommandHandler<TCommand>(
    IMediator mediator,
    IRequestManager requestManager,
    ILogger<IdentifiedCommandHandler<TCommand>> logger)
    : ICommandHandler<IdentifiedCommand<TCommand>>
    where TCommand : ICommand
{
    public async Task Handle(IdentifiedCommand<TCommand> command, CancellationToken ct)
    {
        if (await requestManager.ExistAsync(command.Id, ct))
        {
            logger.LogWarning("Requisição duplicada detectada: {RequestId}", command.Id);
            throw new CampaignDomainException($"A requisição {command.Id} já foi processada.");
        }

        await requestManager.CreateRequestForCommandAsync<TCommand>(command.Id, ct);
        await mediator.Send(command.Command, ct);
    }
}
