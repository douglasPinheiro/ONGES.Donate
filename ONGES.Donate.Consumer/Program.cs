using ONGES.Donate.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<ONGES.Donate.Consumer.Consumers.DonationRequestedConsumer>();

var host = builder.Build();
host.Run();
