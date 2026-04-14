using MassTransit;
using Microsoft.Extensions.Options;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Infrastructure.Configuration;

namespace ONGES.Donate.Infrastructure.Messaging;

public sealed class InternalDonationMessagePublisher(
    ISendEndpointProvider sendEndpointProvider,
    IOptions<MessageBrokerOptions> options) : IInternalDonationMessagePublisher
{
    public async Task PublishAsync(DonationRequestedMessage message, CancellationToken cancellationToken = default)
    {
        var endpoint = await sendEndpointProvider.GetSendEndpoint(
            new Uri($"queue:{options.Value.DonationsQueue}"));

        await endpoint.Send(message, context =>
        {
            context.MessageId = message.DonationId;
        }, cancellationToken);
    }
}
