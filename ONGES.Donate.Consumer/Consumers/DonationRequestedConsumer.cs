using MassTransit;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.Interfaces;

namespace ONGES.Donate.Consumer.Consumers;

public sealed class DonationRequestedConsumer(
    IDonationMessageProcessor donationMessageProcessor,
    ILogger<DonationRequestedConsumer> logger) : IConsumer<DonationRequestedMessage>
{
    public async Task Consume(ConsumeContext<DonationRequestedMessage> context)
    {
        var result = await donationMessageProcessor.ProcessAsync(context.Message, context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Doacao processada com sucesso. DonationId={DonationId} CampaignId={CampaignId}",
                context.Message.DonationId,
                context.Message.CampaignId);

            return;
        }

        logger.LogError(
            "Falha ao processar doacao. DonationId={DonationId} Error={Error}",
            context.Message.DonationId,
            result.Error?.Message);

        throw new InvalidOperationException(result.Error?.Message ?? "Falha ao processar doacao.");
    }
}
