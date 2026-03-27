var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.AddRabbitMqEventBus(builder.Configuration.GetConnectionString("RabbitMQ")!, "EventBus")
    .AddSubscription<DonationIntentProcessingIntegrationEvent, DonationIntentProcessingIntegrationEventHandler>();

builder.Services.AddOptions<DonationOptions>()
    .BindConfiguration(nameof(DonationOptions));

var host = builder.Build();

host.Run();