namespace Campaign.UnitTests.Application.Commands;

public sealed class IdentifiedCommandHandlerTests
{
    [Fact]
    public async Task GenericHandler_WithNewRequest_ShouldCreateRequestAndSendCommand()
    {
        var mediator = Substitute.For<IMediator>();
        var requestManager = Substitute.For<IRequestManager>();
        requestManager.ExistAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        mediator.Send(Arg.Any<TestResponseCommand>(), Arg.Any<CancellationToken>()).Returns("ok");

        var handler = new IdentifiedCommandHandler<TestResponseCommand, string>(
            mediator,
            requestManager,
            NullLogger<IdentifiedCommandHandler<TestResponseCommand, string>>.Instance);

        var result = await handler.Handle(
            new IdentifiedCommand<TestResponseCommand, string>(new TestResponseCommand("payload"), Guid.NewGuid()),
            CancellationToken.None);

        result.Should().Be("ok");
        await requestManager.Received(1).CreateRequestForCommandAsync<TestResponseCommand>(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenericHandler_WithDuplicateRequest_ShouldThrow()
    {
        var mediator = Substitute.For<IMediator>();
        var requestManager = Substitute.For<IRequestManager>();
        requestManager.ExistAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var handler = new IdentifiedCommandHandler<TestResponseCommand, string>(
            mediator,
            requestManager,
            NullLogger<IdentifiedCommandHandler<TestResponseCommand, string>>.Instance);

        var act = async () => await handler.Handle(
            new IdentifiedCommand<TestResponseCommand, string>(new TestResponseCommand("payload"), Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<CampaignDomainException>()
            .WithMessage("A requisição * já foi processada.");
    }

    [Fact]
    public async Task VoidHandler_WithNewRequest_ShouldCreateRequestAndSendCommand()
    {
        var mediator = Substitute.For<IMediator>();
        var requestManager = Substitute.For<IRequestManager>();
        requestManager.ExistAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var handler = new IdentifiedCommandHandler<TestVoidCommand>(
            mediator,
            requestManager,
            NullLogger<IdentifiedCommandHandler<TestVoidCommand>>.Instance);

        await handler.Handle(
            new IdentifiedCommand<TestVoidCommand>(new TestVoidCommand("payload"), Guid.NewGuid()),
            CancellationToken.None);

        await mediator.Received(1).Send(Arg.Any<TestVoidCommand>(), Arg.Any<CancellationToken>());
    }

    public sealed record TestResponseCommand(string Value) : ICommand<string>;
    public sealed record TestVoidCommand(string Value) : ICommand;
}
