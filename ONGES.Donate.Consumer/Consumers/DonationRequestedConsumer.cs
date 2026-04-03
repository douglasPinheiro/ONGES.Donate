using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Infrastructure.Configuration;
using System.Text.Json;

namespace ONGES.Donate.Consumer.Consumers;

public sealed class DonationRequestedConsumer(
    ServiceBusClient serviceBusClient,
    IOptions<ServiceBusOptions> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DonationRequestedConsumer> logger) : BackgroundService
{
    private ServiceBusProcessor? _processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = serviceBusClient.CreateProcessor(
            options.Value.DonationsTopic,
            options.Value.DonationsSubscription,
            new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var payload = args.Message.Body.ToString();
        var message = JsonSerializer.Deserialize<DonationRequestedMessage>(payload);

        if (message is null)
        {
            await args.DeadLetterMessageAsync(args.Message, cancellationToken: args.CancellationToken);
            return;
        }

        using var scope = serviceScopeFactory.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<IDonationMessageProcessor>();
        var result = await processor.ProcessAsync(message, args.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Doacao processada com sucesso. DonationId={DonationId} CampaignId={CampaignId}",
                message.DonationId,
                message.CampaignId);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        logger.LogError(
            "Falha ao processar doacao. DonationId={DonationId} Error={Error}",
            message.DonationId,
            result.Error?.Message);

        await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(
            args.Exception,
            "Erro no consumer de doacoes. Entity={EntityPath} Source={ErrorSource}",
            args.EntityPath,
            args.ErrorSource);

        return Task.CompletedTask;
    }
}
