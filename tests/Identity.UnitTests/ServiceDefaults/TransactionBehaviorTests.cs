namespace Identity.UnitTests.ServiceDefaults;

public sealed class TransactionBehaviorTests
{
    [Fact]
    public async Task GenericTransactionBehavior_WithActiveTransaction_ShouldSkipTransactionHandling()
    {
        var context = new FakeTransactionalContext { HasActiveTransaction = true };
        var publisher = Substitute.For<ITransactionEventPublisher>();
        var behavior = new TransactionBehavior<TestResponseCommand, string>(
            context,
            [publisher],
            NullLogger<TransactionBehavior<TestResponseCommand, string>>.Instance);
        var nextCalled = false;

        var result = await behavior.Handle(
            new TestResponseCommand("value"),
            _ =>
            {
                nextCalled = true;
                return Task.FromResult("ok");
            },
            CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().Be("ok");
        await publisher.DidNotReceive().PublishAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenericTransactionBehavior_ShouldCommitAndPublish()
    {
        var context = new FakeTransactionalContext();
        var publisher = Substitute.For<ITransactionEventPublisher>();
        var behavior = new TransactionBehavior<TestResponseCommand, string>(
            context,
            [publisher],
            NullLogger<TransactionBehavior<TestResponseCommand, string>>.Instance);

        var result = await behavior.Handle(
            new TestResponseCommand("value"),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        result.Should().Be("ok");
        context.BeginCalled.Should().BeTrue();
        context.CommitCalled.Should().BeTrue();
        await publisher.Received(1).PublishAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task VoidTransactionBehavior_WhenTransactionIsNull_ShouldStillExecuteNext()
    {
        var context = new FakeTransactionalContext { ReturnNullTransaction = true };
        var publisher = Substitute.For<ITransactionEventPublisher>();
        var behavior = new TransactionBehavior<TestVoidCommand>(
            context,
            [publisher],
            NullLogger<TransactionBehavior<TestVoidCommand>>.Instance);
        var nextCalled = false;

        await behavior.Handle(
            new TestVoidCommand("value"),
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            CancellationToken.None);

        nextCalled.Should().BeTrue();
        context.CommitCalled.Should().BeFalse();
        await publisher.DidNotReceive().PublishAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenericTransactionBehavior_WhenTransactionIsNull_ShouldStillExecuteNext()
    {
        var context = new FakeTransactionalContext { ReturnNullTransaction = true };
        var publisher = Substitute.For<ITransactionEventPublisher>();
        var behavior = new TransactionBehavior<TestResponseCommand, string>(
            context,
            [publisher],
            NullLogger<TransactionBehavior<TestResponseCommand, string>>.Instance);

        var result = await behavior.Handle(
            new TestResponseCommand("value"),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        result.Should().Be("ok");
        context.CommitCalled.Should().BeFalse();
        await publisher.DidNotReceive().PublishAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenericTransactionBehavior_WhenNextThrows_ShouldBubbleException()
    {
        var context = new FakeTransactionalContext();
        var publisher = Substitute.For<ITransactionEventPublisher>();
        var behavior = new TransactionBehavior<TestResponseCommand, string>(
            context,
            [publisher],
            NullLogger<TransactionBehavior<TestResponseCommand, string>>.Instance);

        var act = async () => await behavior.Handle(
            new TestResponseCommand("value"),
            _ => throw new InvalidOperationException("boom"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task VoidTransactionBehavior_WithActiveTransaction_ShouldSkipTransactionHandling()
    {
        var context = new FakeTransactionalContext { HasActiveTransaction = true };
        var publisher = Substitute.For<ITransactionEventPublisher>();
        var behavior = new TransactionBehavior<TestVoidCommand>(
            context,
            [publisher],
            NullLogger<TransactionBehavior<TestVoidCommand>>.Instance);
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
        context.BeginCalled.Should().BeFalse();
        await publisher.DidNotReceive().PublishAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task VoidTransactionBehavior_ShouldCommitAndPublish()
    {
        var context = new FakeTransactionalContext();
        var publisher = Substitute.For<ITransactionEventPublisher>();
        var behavior = new TransactionBehavior<TestVoidCommand>(
            context,
            [publisher],
            NullLogger<TransactionBehavior<TestVoidCommand>>.Instance);

        await behavior.Handle(
            new TestVoidCommand("value"),
            _ => Task.CompletedTask,
            CancellationToken.None);

        context.BeginCalled.Should().BeTrue();
        context.CommitCalled.Should().BeTrue();
        await publisher.Received(1).PublishAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    public sealed record TestResponseCommand(string Value) : ICommand<string>;
    public sealed record TestVoidCommand(string Value) : ICommand;

    private sealed class FakeTransactionalContext : ITransactionalContext
    {
        private readonly IdentityContext _dbContext;

        public FakeTransactionalContext()
        {
            var options = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new IdentityContext(options);
        }

        public bool HasActiveTransaction { get; set; }
        public bool ReturnNullTransaction { get; set; }
        public bool BeginCalled { get; private set; }
        public bool CommitCalled { get; private set; }

        public IExecutionStrategy CreateExecutionStrategy() => _dbContext.Database.CreateExecutionStrategy();

        public Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            BeginCalled = true;

            if (ReturnNullTransaction)
                return Task.FromResult<IDbContextTransaction?>(null);

            HasActiveTransaction = true;
            return Task.FromResult<IDbContextTransaction?>(new FakeDbContextTransaction());
        }

        public Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            CommitCalled = true;
            HasActiveTransaction = false;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeDbContextTransaction : IDbContextTransaction
    {
        public Guid TransactionId { get; } = Guid.NewGuid();

        public void Dispose() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public void Commit() { }

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Rollback() { }

        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public IDbContextTransaction GetDbTransaction() => throw new NotSupportedException();

        public void CreateSavepoint(string name) { }

        public Task CreateSavepointAsync(string name, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void RollbackToSavepoint(string name) { }

        public Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void ReleaseSavepoint(string name) { }

        public Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
