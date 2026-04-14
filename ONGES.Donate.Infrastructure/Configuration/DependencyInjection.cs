using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ONGES.Donate.Application.DTOs.Requests;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Application.Services;
using ONGES.Donate.Infrastructure.Messaging;
using ONGES.Donate.Infrastructure.Persistence;
using ONGES.Donate.Infrastructure.Repositories;
using ONGES.Donate.Infrastructure.Services;
using ONGES.Donate.Infrastructure.Validators;

namespace ONGES.Donate.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureMassTransitRegistration = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureRabbitMq = null)
    {
        services.Configure<MessageBrokerOptions>(configuration.GetSection(MessageBrokerOptions.SectionName));
        services.Configure<CampaignApiOptions>(configuration.GetSection(CampaignApiOptions.SectionName));

        services.AddDbContext<DonateDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IDonationRepository, DonationRepository>();
        services.AddScoped<IDonationService, DonationService>();
        services.AddScoped<IDonationMessageProcessor, DonationMessageProcessor>();
        services.AddScoped<IValidator<CreateDonationRequest>, CreateDonationRequestValidator>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            configureMassTransitRegistration?.Invoke(busConfigurator);

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<MessageBrokerOptions>>().Value;
                cfg.Host(options.Host, options.Port, options.VirtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(options.Username);
                    hostConfigurator.Password(options.Password);
                });
                cfg.UseRawJsonSerializer();
                cfg.UseRawJsonDeserializer();

                configureRabbitMq?.Invoke(context, cfg);

                if (configureRabbitMq is null)
                    cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IInternalDonationMessagePublisher, InternalDonationMessagePublisher>();
        services.AddScoped<ICampaignUpdatePublisher, CampaignUpdatePublisher>();

        services.AddHttpClient<ICampaignValidationGateway, CampaignValidationGateway>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CampaignApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}
