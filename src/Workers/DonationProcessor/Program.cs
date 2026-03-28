var builder = Host.CreateApplicationBuilder(args);

builder.AddRabbitMqEventBus(builder.Configuration.GetConnectionString("RabbitMQ")!, "donation-processor")
    .AddSubscription<DonationIntentProcessingIntegrationEvent, DonationIntentProcessingIntegrationEventHandler>();

builder.Services.AddOptions<DonationOptions>()
    .BindConfiguration(nameof(DonationOptions));

var host = builder.Build();

host.Run();
