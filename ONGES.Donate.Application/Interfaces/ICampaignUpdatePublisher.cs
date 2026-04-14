using ONGES.Contracts.DTOs;

namespace ONGES.Donate.Application.Interfaces;

public interface ICampaignUpdatePublisher
{
    Task PublishAsync(DonationMessage message, CancellationToken cancellationToken = default);
}
