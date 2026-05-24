using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.Grafana.Loki;

namespace ServiceDefaults.Observability;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        ConfigureOpenTelemetry(builder);
        ConfigureSerilog(builder);

        return builder;
    }

    public static IHostApplicationBuilder AddObservability(this IHostApplicationBuilder builder)
    {
        ConfigureOpenTelemetry(builder);
        ConfigureSerilog(builder);

        return builder;
    }

    public static WebApplicationBuilder AddPrometheusMetrics(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddPrometheusExporter();
            });

        return builder;
    }

    public static IHostApplicationBuilder AddPrometheusMetrics(this IHostApplicationBuilder builder)
    {
        var scrapePath = builder.Configuration["Prometheus:ScrapeEndpointPath"] ?? "/metrics";
        var uriPrefixes = builder.Configuration.GetSection("Prometheus:UriPrefixes")
            .GetChildren()
            .Select(child => child.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Cast<string>()
            .ToArray();

        if (uriPrefixes.Length == 0)
        {
            uriPrefixes = ["http://localhost:9464/"];
        }

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddPrometheusHttpListener(options =>
                {
                    options.ScrapeEndpointPath = scrapePath;
                    options.UriPrefixes = uriPrefixes;
                });
            });

        return builder;
    }

    public static WebApplication UsePrometheusMetrics(this WebApplication app)
    {
        var scrapePath = app.Configuration["Prometheus:ScrapeEndpointPath"] ?? "/metrics";
        app.MapPrometheusScrapingEndpoint(scrapePath).AllowAnonymous();

        return app;
    }

    private static void ConfigureOpenTelemetry(IHostApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: builder.Environment.ApplicationName,
                serviceNamespace: "solidarityhub",
                serviceVersion: "1.0.0");

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: builder.Environment.ApplicationName,
                serviceNamespace: "solidarityhub",
                serviceVersion: "1.0.0"))
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(builder.Environment.ApplicationName)
                    .AddSource(RabbitMQTelemetry.ActivitySourceName);

                var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
                if (Uri.TryCreate(otlpEndpoint, UriKind.Absolute, out var endpoint))
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = endpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
                }
            });
    }

    private static void ConfigureSerilog(IHostApplicationBuilder builder)
    {
        var lokiUri = builder.Configuration.GetConnectionString("Loki") ?? "http://loki:3100";

        builder.Services.AddSerilog(loggerConfiguration =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .Enrich.WithSpan()
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.GrafanaLoki(lokiUri, labels:
                [
                    new LokiLabel { Key = "app", Value = builder.Environment.ApplicationName },
                    new LokiLabel { Key = "environment", Value = builder.Environment.EnvironmentName }
                ]);
        });
    }
}
