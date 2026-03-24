namespace Identity.UnitTests.ServiceDefaults;

public sealed class IdempotencyBehaviorTests
{
    [Fact]
    public async Task GenericBehavior_WithNonIdempotentCommand_ShouldPassThrough()
    {
        var requestManager = Substitute.For<IRequestManager>();
        var behavior = new IdempotencyBehavior<NonIdempotentResponseCommand, string>(
            requestManager,
            NullLogger<IdempotencyBehavior<NonIdempotentResponseCommand, string>>.Instance);

        var result = await behavior.Handle(
            new NonIdempotentResponseCommand("value"),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        result.Should().Be("ok");
        await requestManager.DidNotReceive()
            .CreateRequestForCommandAsync<NonIdempotentResponseCommand>(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenericBehavior_WithEmptyRequestId_ShouldPassThrough()
    {
        var requestManager = Substitute.For<IRequestManager>();
        var behavior = new IdempotencyBehavior<IdempotentResponseCommand, string>(
            requestManager,
            NullLogger<IdempotencyBehavior<IdempotentResponseCommand, string>>.Instance);

        var result = await behavior.Handle(
            new IdempotentResponseCommand("value"),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        result.Should().Be("ok");
        await requestManager.DidNotReceive()
            .CreateRequestForCommandAsync<IdempotentResponseCommand>(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenericBehavior_WithNewRequest_ShouldCreateRequestAndContinue()
    {
        var requestManager = Substitute.For<IRequestManager>();
        requestManager.ExistAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var behavior = new IdempotencyBehavior<IdempotentResponseCommand, string>(
            requestManager,
            NullLogger<IdempotencyBehavior<IdempotentResponseCommand, string>>.Instance);

        var request = new IdempotentResponseCommand("value", Guid.NewGuid());

        var result = await behavior.Handle(
            request,
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        result.Should().Be("ok");
        await requestManager.Received(1)
            .CreateRequestForCommandAsync<IdempotentResponseCommand>(request.RequestId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenericBehavior_WithDuplicateRequest_ShouldThrow()
    {
        var requestManager = Substitute.For<IRequestManager>();
        requestManager.ExistAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var behavior = new IdempotencyBehavior<IdempotentResponseCommand, string>(
            requestManager,
            NullLogger<IdempotencyBehavior<IdempotentResponseCommand, string>>.Instance);

        var request = new IdempotentResponseCommand("value", Guid.NewGuid());

        var act = async () => await behavior.Handle(
            request,
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        await act.Should().ThrowAsync<DuplicateRequestException>()
            .WithMessage("A requisição * já foi processada.");
    }

    [Fact]
    public async Task VoidBehavior_WithNewRequest_ShouldCreateRequestAndContinue()
    {
        var requestManager = Substitute.For<IRequestManager>();
        requestManager.ExistAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var behavior = new IdempotencyBehavior<IdempotentVoidCommand>(
            requestManager,
            NullLogger<IdempotencyBehavior<IdempotentVoidCommand>>.Instance);

        var request = new IdempotentVoidCommand("value", Guid.NewGuid());
        var called = false;

        await behavior.Handle(
            request,
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            CancellationToken.None);

        called.Should().BeTrue();
        await requestManager.Received(1)
            .CreateRequestForCommandAsync<IdempotentVoidCommand>(request.RequestId, Arg.Any<CancellationToken>());
    }

    public sealed record NonIdempotentResponseCommand(string Value) : ICommand<string>;

    public sealed record IdempotentResponseCommand(string Value, Guid RequestId = default) : IIdempotentCommand<string>;

    public sealed record IdempotentVoidCommand(string Value, Guid RequestId = default) : IIdempotentCommand;
}
