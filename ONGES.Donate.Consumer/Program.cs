using ONGES.Donate.Infrastructure.Configuration;
using ONGES.Donate.Consumer.Consumers;
using Microsoft.Extensions.Options;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(
    builder.Configuration,
    configureMassTransitRegistration: busConfigurator =>
    {
        busConfigurator.AddConsumer<DonationRequestedConsumer>();
    },
    configureRabbitMq: (context, cfg) =>
    {
        var options = context.GetRequiredService<IOptions<MessageBrokerOptions>>().Value;

        cfg.ReceiveEndpoint(options.DonationsQueue, endpoint =>
        {
            endpoint.ConfigureConsumer<DonationRequestedConsumer>(context);
        });
    });

var host = builder.Build();
host.Run();
