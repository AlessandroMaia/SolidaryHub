using System.Collections;

namespace Identity.UnitTests.ServiceDefaults;

public sealed class AdditionalCoverageTests
{
    [Fact]
    public async Task ExceptionHandlingMiddleware_WhenNoExceptionOccurs_ShouldInvokeNext()
    {
        var called = false;
        var middleware = new ExceptionHandlingMiddleware(
            context =>
            {
                called = true;
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return Task.CompletedTask;
            },
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(new DefaultHttpContext());

        called.Should().BeTrue();
    }

    [Fact]
    public async Task BearerSecuritySchemeTransformer_WithNullPaths_ShouldReturnAfterConfiguringSecurityScheme()
    {
        var provider = Substitute.For<IAuthenticationSchemeProvider>();
        provider.GetAllSchemesAsync().Returns([new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, null, typeof(IAuthenticationHandler))]);
        var transformer = new BearerSecuritySchemeTransformer(provider);
        var document = new OpenApiDocument
        {
            Paths = null!
        };

        await transformer.TransformAsync(document, null!, CancellationToken.None);

        document.Components.Should().NotBeNull();
        document.Components!.SecuritySchemes.Should().ContainKey(JwtBearerDefaults.AuthenticationScheme);
    }

    [Fact]
    public async Task BearerSecuritySchemeTransformer_WithNullOperation_ShouldReturnAfterConfiguringSecurityScheme()
    {
        var provider = Substitute.For<IAuthenticationSchemeProvider>();
        provider.GetAllSchemesAsync().Returns([new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, null, typeof(IAuthenticationHandler))]);
        var transformer = new BearerSecuritySchemeTransformer(provider);
        var pathItem = new OpenApiPathItem();
        var operationsProperty = typeof(OpenApiPathItem).GetProperty(nameof(OpenApiPathItem.Operations))!;
        var dictionary = (IDictionary)Activator.CreateInstance(operationsProperty.PropertyType)!;
        dictionary.Add(System.Net.Http.HttpMethod.Get, null);
        operationsProperty.SetValue(pathItem, dictionary);
        var document = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/null"] = pathItem
            }
        };

        await transformer.TransformAsync(document, null!, CancellationToken.None);

        document.Components.Should().NotBeNull();
    }

    [Fact]
    public async Task VoidTransactionBehavior_WhenNextThrows_ShouldBubbleException()
    {
        var context = new FakeTransactionalContext();
        var publishers = new[] { Substitute.For<ITransactionEventPublisher>() };
        var behavior = new TransactionBehavior<TestVoidCommand>(
            context,
            publishers,
            NullLogger<TransactionBehavior<TestVoidCommand>>.Instance);

        var act = async () => await behavior.Handle(
            new TestVoidCommand("value"),
            _ => throw new InvalidOperationException("boom"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

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

        public bool HasActiveTransaction => false;

        public IExecutionStrategy CreateExecutionStrategy() => _dbContext.Database.CreateExecutionStrategy();

        public Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IDbContextTransaction?>(new FakeDbContextTransaction());

        public Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
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
