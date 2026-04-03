using ONGES.Donate.Application.DTOs.Messages;

namespace ONGES.Donate.Application.Interfaces;

public interface ICampaignUpdatePublisher
{
    Task PublishAsync(UpdateCampaignDonationMessage message, CancellationToken cancellationToken = default);
}
