using MassTransit;
using Microsoft.Extensions.Options;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Infrastructure.Configuration;
using ONGES.Contracts.DTOs;

namespace ONGES.Donate.Infrastructure.Messaging;

public sealed class CampaignUpdatePublisher(
    ISendEndpointProvider sendEndpointProvider,
    IOptions<MessageBrokerOptions> options) : ICampaignUpdatePublisher
{
    public async Task PublishAsync(DonationMessage message, CancellationToken cancellationToken = default)
    {
        var endpoint = await sendEndpointProvider.GetSendEndpoint(
            new Uri($"queue:{options.Value.CampaignUpdatesQueue}"));

        await endpoint.Send(message, context =>
        {
            context.MessageId = NewId.NextGuid();
        }, cancellationToken);
    }
}
