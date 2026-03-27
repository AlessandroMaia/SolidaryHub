namespace Mediator.Commands;

public interface IIdempotentCommand : ICommand
{
    Guid RequestId { get; }
}

public interface IIdempotentCommand<TResponse> : ICommand<TResponse>
{
    Guid RequestId { get; }
}
