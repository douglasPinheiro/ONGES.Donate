using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Infrastructure.Configuration;
using System.Text.Json;

namespace ONGES.Donate.Infrastructure.Messaging;

public sealed class CampaignUpdatePublisher(
    ServiceBusClient serviceBusClient,
    IOptions<ServiceBusOptions> options) : ICampaignUpdatePublisher
{
    public async Task PublishAsync(UpdateCampaignDonationMessage message, CancellationToken cancellationToken = default)
    {
        var sender = serviceBusClient.CreateSender(options.Value.CampaignUpdatesEntity);

        await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(new
        {
            CampaignId = message.CampaignId,
            Amount = message.Amount,
            DonatedAt = message.DonatedAt
        }))
        {
            Subject = nameof(UpdateCampaignDonationMessage),
            ContentType = "application/json",
            MessageId = $"{message.CampaignId:N}-{message.DonatedAt:O}"
        }, cancellationToken);

        await sender.DisposeAsync();
    }
}
