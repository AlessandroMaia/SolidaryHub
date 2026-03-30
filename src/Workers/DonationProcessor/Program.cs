using DonationProcessor.Observability;
using OpenTelemetry.Metrics;
using ServiceDefaults.Observability;

var builder = Host.CreateApplicationBuilder(args);

builder.AddObservability()
    .AddPrometheusMetrics();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics.AddMeter(DonationProcessorMetrics.MeterName));

builder.AddRabbitMqEventBus(builder.Configuration.GetConnectionString("RabbitMQ")!, "donation-processor")
    .AddSubscription<DonationIntentProcessingIntegrationEvent, DonationIntentProcessingIntegrationEventHandler>();

builder.Services.AddOptions<DonationOptions>()
    .BindConfiguration(nameof(DonationOptions));

var host = builder.Build();

host.Run();
