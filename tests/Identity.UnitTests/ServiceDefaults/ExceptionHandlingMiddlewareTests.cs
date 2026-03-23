using System.Text.Json;

namespace Identity.UnitTests.ServiceDefaults;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenValidationExceptionOccurs_ShouldReturnBadRequest()
    {
        var middleware = CreateMiddleware(_ => throw new ValidationException([new FluentValidation.Results.ValidationFailure("Email", "Inválido")]));
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        var json = await ReadJsonAsync(context);
        json.RootElement.GetProperty("title").GetString().Should().Be("Validation Error");
    }

    [Fact]
    public async Task InvokeAsync_WhenDomainExceptionOccurs_ShouldReturnBadRequest()
    {
        var middleware = CreateMiddleware(_ => throw new IdentityDomainException("Regra inválida"));
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        var json = await ReadJsonAsync(context);
        json.RootElement.GetProperty("detail").GetString().Should().Be("Regra inválida");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedOccurs_ShouldReturnUnauthorized()
    {
        var middleware = CreateMiddleware(_ => throw new UnauthorizedAccessException());
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnexpectedExceptionOccurs_ShouldReturnServerError()
    {
        var middleware = CreateMiddleware(_ => throw new InvalidOperationException("boom"));
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_WhenLoggerIsEnabled_ShouldWriteErrorLog()
    {
        var logger = new TestLogger<ExceptionHandlingMiddleware>();
        var middleware = new ExceptionHandlingMiddleware(_ => throw new InvalidOperationException("boom"), logger);
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        logger.Messages.Should().ContainSingle(message => message.Contains("InvalidOperationException"));
    }

    [Fact]
    public void UseExceptionHandling_ShouldReturnSameBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        var returned = app.UseExceptionHandling();

        returned.Should().BeSameAs(app);
    }

    private static ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate next)
        => new(next, NullLogger<ExceptionHandlingMiddleware>.Instance);

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<JsonDocument> ReadJsonAsync(DefaultHttpContext context)
    {
        context.Response.Body.Position = 0;
        return await JsonDocument.ParseAsync(context.Response.Body);
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
