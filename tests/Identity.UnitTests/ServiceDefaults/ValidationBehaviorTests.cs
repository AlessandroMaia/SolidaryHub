namespace Identity.UnitTests.ServiceDefaults;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task GenericValidationBehavior_WithValidRequest_ShouldInvokeNext()
    {
        var behavior = new ValidationBehavior<TestResponseCommand, string>([new TestResponseValidator()]);
        var nextCalled = false;

        var result = await behavior.Handle(
            new TestResponseCommand("ok"),
            _ =>
            {
                nextCalled = true;
                return Task.FromResult("done");
            },
            CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().Be("done");
    }

    [Fact]
    public async Task GenericValidationBehavior_WithInvalidRequest_ShouldThrow()
    {
        var behavior = new ValidationBehavior<TestResponseCommand, string>([new TestResponseValidator()]);

        var act = async () => await behavior.Handle(
            new TestResponseCommand(""),
            _ => Task.FromResult("done"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task VoidValidationBehavior_WithInvalidRequest_ShouldThrow()
    {
        var behavior = new ValidationBehavior<TestVoidCommand>([new TestVoidValidator()]);

        var act = async () => await behavior.Handle(
            new TestVoidCommand(""),
            _ => Task.CompletedTask,
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task VoidValidationBehavior_WithValidRequest_ShouldInvokeNext()
    {
        var behavior = new ValidationBehavior<TestVoidCommand>([new TestVoidValidator()]);
        var called = false;

        await behavior.Handle(
            new TestVoidCommand("ok"),
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            CancellationToken.None);

        called.Should().BeTrue();
    }

    [Fact]
    public async Task GenericValidationBehavior_WithoutValidators_ShouldInvokeNext()
    {
        var behavior = new ValidationBehavior<TestResponseCommand, string>([]);

        var result = await behavior.Handle(
            new TestResponseCommand("ok"),
            _ => Task.FromResult("done"),
            CancellationToken.None);

        result.Should().Be("done");
    }

    public sealed record TestResponseCommand(string Value) : ICommand<string>;
    public sealed record TestVoidCommand(string Value) : ICommand;

    private sealed class TestResponseValidator : AbstractValidator<TestResponseCommand>
    {
        public TestResponseValidator()
        {
            RuleFor(x => x.Value).NotEmpty();
        }
    }

    private sealed class TestVoidValidator : AbstractValidator<TestVoidCommand>
    {
        public TestVoidValidator()
        {
            RuleFor(x => x.Value).NotEmpty();
        }
    }
}
