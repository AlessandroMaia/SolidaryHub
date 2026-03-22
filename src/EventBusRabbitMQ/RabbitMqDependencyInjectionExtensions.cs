namespace EventBusRabbitMQ;

public static class RabbitMqDependencyInjectionExtensions
{
    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }

    public static IEventBusBuilder AddRabbitMqEventBus(
        this IHostApplicationBuilder builder,
        string connectionString,
        string subscriptionClientName)
    {
        builder.Services.AddOpenTelemetry()
           .WithTracing(tracing =>
           {
               tracing.AddSource(RabbitMQTelemetry.ActivitySourceName);
           });

        builder.Services.AddSingleton(sp =>
        {
            var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("RabbitMQ");

            const int maxRetries = 10;
            const int baseDelaySeconds = 2;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation("Conectando ao RabbitMQ (tentativa {Attempt}/{Max})...", attempt, maxRetries);

                    var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

                    if(logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation("Conexão com o RabbitMQ estabelecida com sucesso");

                    return connection;
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    var delay = TimeSpan.FromSeconds(baseDelaySeconds * attempt);
                    logger.LogWarning(ex, "RabbitMQ ainda não está disponível. Nova tentativa em {Delay}s ({Attempt}/{Max})...",
                        delay.TotalSeconds, attempt, maxRetries);
                    Thread.Sleep(delay);
                }
            }

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        builder.Services.Configure<EventBusOptions>(o =>
        {
            o.SubscriptionClientName = subscriptionClientName;
        });

        builder.Services.AddSingleton<RabbitMQTelemetry>();
        builder.Services.AddSingleton<RabbitMQEventBus>();
        builder.Services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<RabbitMQEventBus>());
        builder.Services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<RabbitMQEventBus>());

        return new EventBusBuilder(builder.Services);
    }
}
