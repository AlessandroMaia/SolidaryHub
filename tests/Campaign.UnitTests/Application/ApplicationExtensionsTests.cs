using Campaign.API.Extensions;
using Mediator.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.UnitTests.Application;

public sealed class ApplicationExtensionsTests
{
    [Fact]
    public void AddApplication_ShouldRegisterCommandPipelineBehaviorsInExpectedOrder()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<,>))
            .Select(d => d.ImplementationType)
            .Should()
            .ContainInOrder(
            [
                typeof(LoggingBehavior<,>),
                typeof(ValidationBehavior<,>),
                typeof(TransactionBehavior<,>),
                typeof(IdempotencyBehavior<,>)
            ]);

        services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<>))
            .Select(d => d.ImplementationType)
            .Should()
            .ContainInOrder(
            [
                typeof(LoggingBehavior<>),
                typeof(ValidationBehavior<>),
                typeof(TransactionBehavior<>),
                typeof(IdempotencyBehavior<>)
            ]);
    }
}
