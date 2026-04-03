using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Infrastructure.Configuration;
using System.Text.Json;

namespace ONGES.Donate.Infrastructure.Messaging;

public sealed class InternalDonationMessagePublisher(
    ServiceBusClient serviceBusClient,
    IOptions<ServiceBusOptions> options) : IInternalDonationMessagePublisher
{
    public async Task PublishAsync(DonationRequestedMessage message, CancellationToken cancellationToken = default)
    {
        var sender = serviceBusClient.CreateSender(options.Value.DonationsTopic);

        await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(message))
        {
            Subject = nameof(DonationRequestedMessage),
            ContentType = "application/json",
            MessageId = message.DonationId.ToString()
        }, cancellationToken);

        await sender.DisposeAsync();
    }
}
