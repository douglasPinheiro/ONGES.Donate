using Azure.Messaging.ServiceBus;
using FluentValidation;
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
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.SectionName));
        services.Configure<CampaignApiOptions>(configuration.GetSection(CampaignApiOptions.SectionName));

        services.AddDbContext<DonateDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IDonationRepository, DonationRepository>();
        services.AddScoped<IDonationService, DonationService>();
        services.AddScoped<IDonationMessageProcessor, DonationMessageProcessor>();
        services.AddScoped<IValidator<CreateDonationRequest>, CreateDonationRequestValidator>();

        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
            return new ServiceBusClient(options.ConnectionString);
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
