using ONGES.Donate.Application.DTOs.Messages;

namespace ONGES.Donate.Application.Interfaces;

public interface IInternalDonationMessagePublisher
{
    Task PublishAsync(DonationRequestedMessage message, CancellationToken cancellationToken = default);
}
