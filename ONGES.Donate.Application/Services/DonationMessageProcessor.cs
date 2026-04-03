using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Domain.Entities;
using ONGES.Donate.Domain.Shared;

namespace ONGES.Donate.Application.Services;

public sealed class DonationMessageProcessor(
    IDonationRepository donationRepository,
    ICampaignUpdatePublisher campaignUpdatePublisher) : IDonationMessageProcessor
{
    public async Task<Result> ProcessAsync(DonationRequestedMessage message, CancellationToken cancellationToken = default)
    {
        if (await donationRepository.ExistsAsync(message.DonationId, cancellationToken))
            return Result.Success();

        var donation = DonationEntity.Create(
            message.DonationId,
            message.CampaignId,
            message.DonorUserId,
            message.Amount,
            message.RequestedAt);

        donation.MarkAsProcessed(DateTime.UtcNow);

        await donationRepository.AddAsync(donation, cancellationToken);
        await donationRepository.SaveChangesAsync(cancellationToken);

        await campaignUpdatePublisher.PublishAsync(
            new UpdateCampaignDonationMessage(
                message.CampaignId,
                message.Amount,
                donation.ProcessedAt ?? DateTime.UtcNow),
            cancellationToken);

        return Result.Success();
    }
}
