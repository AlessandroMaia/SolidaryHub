namespace Identity.UnitTests.SharedKernel;

public sealed class AdvancedCoreTests
{
    [Fact]
    public void ValidationError_ShouldKeepOnlyFailureErrors()
    {
        Result[] results =
        [
            Result.Success(),
            Result.Failure(new Error("VALIDATION", "Inválido", ErrorType.Validation)),
            Result.Failure(Error.Conflict("CONFLICT", "Conflito"))
        ];

        var validationError = ValidationError.FromResults(results);

        validationError.Code.Should().Be("General.Validation");
        validationError.Type.Should().Be(ErrorType.Validation);
        validationError.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void DomainExceptionDerivedTypes_ShouldPreserveMessageAndInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var domain = new TestDomainException("domain", inner);
        var identity = new IdentityDomainException("identity", inner);
        var duplicateRequest = new DuplicateRequestException("duplicate", inner);

        domain.Message.Should().Be("domain");
        domain.InnerException.Should().BeSameAs(inner);
        identity.Message.Should().Be("identity");
        identity.InnerException.Should().BeSameAs(inner);
        duplicateRequest.Message.Should().Be("duplicate");
        duplicateRequest.InnerException.Should().BeSameAs(inner);
        new IdentityDomainException().Message.Should().NotBeNull();
    }

    [Fact]
    public void Entity_ShouldManageDomainEventsAndNullComparisons()
    {
        var entity = new TestEntity();
        var notification = Substitute.For<Mediator.Notifications.INotification>();

        entity.AddDomainEvent(notification);
        entity.DomainEvents.Should().ContainSingle().Which.Should().BeSameAs(notification);

        entity.RemoveDomainEvent(notification);
        entity.DomainEvents.Should().BeEmpty();

        entity.AddDomainEvent(notification);
        entity.ClearDomainEvents();
        entity.DomainEvents.Should().BeEmpty();

        object? nullObject = null;
        TestEntity? nullLeft = null;
        TestEntity? nullRight = null;

        entity.Equals(nullObject).Should().BeFalse();
        entity.Equals((object)entity).Should().BeTrue();
#pragma warning disable CS8604
        (entity == nullLeft).Should().BeFalse();
        (nullLeft == nullRight).Should().BeTrue();
#pragma warning restore CS8604
    }

    [Fact]
    public void Result_ShouldCoverRemainingBranches()
    {
        var success = Result.Success("value");
        Result<string> implicitSuccess = "value";
        var validationFailure = Result<string>.ValidationFailure(
            new Error("VAL", "Falha", ErrorType.Validation));

        success.Value.Should().Be("value");
        implicitSuccess.IsSuccess.Should().BeTrue();
        implicitSuccess.Value.Should().Be("value");
        validationFailure.IsFailure.Should().BeTrue();
        validationFailure.Error.Type.Should().Be(ErrorType.Validation);

        var invalidSuccess = () => new Result(true, Error.Failure("ERR", "Falhou"));
        var invalidFailure = () => new Result(false, Error.None);

        invalidSuccess.Should().Throw<ArgumentException>();
        invalidFailure.Should().Throw<ArgumentException>();
    }

    private sealed class TestEntity : Entity;

    private sealed class TestDomainException : DomainException
    {
        public TestDomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
