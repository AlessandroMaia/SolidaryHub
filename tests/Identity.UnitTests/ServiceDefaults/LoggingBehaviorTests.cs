namespace Identity.UnitTests.ServiceDefaults;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task GenericLoggingBehavior_ShouldReturnResponse()
    {
        var behavior = new LoggingBehavior<TestResponseCommand, string>(NullLogger<LoggingBehavior<TestResponseCommand, string>>.Instance);

        var result = await behavior.Handle(
            new TestResponseCommand("value"),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        result.Should().Be("ok");
    }

    [Fact]
    public async Task GenericLoggingBehavior_WhenNextThrows_ShouldRethrow()
    {
        var behavior = new LoggingBehavior<TestResponseCommand, string>(NullLogger<LoggingBehavior<TestResponseCommand, string>>.Instance);

        var act = async () => await behavior.Handle(
            new TestResponseCommand("value"),
            _ => throw new InvalidOperationException("erro"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task VoidLoggingBehavior_ShouldInvokeNext()
    {
        var behavior = new LoggingBehavior<TestVoidCommand>(NullLogger<LoggingBehavior<TestVoidCommand>>.Instance);
        var called = false;

        await behavior.Handle(
            new TestVoidCommand("value"),
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            CancellationToken.None);

        called.Should().BeTrue();
    }

    [Fact]
    public async Task VoidLoggingBehavior_WhenNextThrows_ShouldRethrow()
    {
        var behavior = new LoggingBehavior<TestVoidCommand>(NullLogger<LoggingBehavior<TestVoidCommand>>.Instance);

        var act = async () => await behavior.Handle(
            new TestVoidCommand("value"),
            _ => throw new InvalidOperationException("erro"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    public sealed record TestResponseCommand(string Value) : ICommand<string>;
    public sealed record TestVoidCommand(string Value) : ICommand;
}
