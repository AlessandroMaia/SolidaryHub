namespace Campaign.API.Application.Commands;

public sealed record IdentifiedCommand<TCommand, TResponse>(TCommand Command, Guid Id)
    : ICommand<TResponse>
    where TCommand : ICommand<TResponse>;

public sealed record IdentifiedCommand<TCommand>(TCommand Command, Guid Id)
    : ICommand
    where TCommand : ICommand;