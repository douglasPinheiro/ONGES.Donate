using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Domain.Shared;

namespace ONGES.Donate.Application.Interfaces;

public interface IDonationMessageProcessor
{
    Task<Result> ProcessAsync(DonationRequestedMessage message, CancellationToken cancellationToken = default);
}
