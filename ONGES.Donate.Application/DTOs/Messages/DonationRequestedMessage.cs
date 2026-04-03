namespace ONGES.Donate.Application.DTOs.Messages;

public sealed record DonationRequestedMessage(
    Guid DonationId,
    Guid CampaignId,
    Guid DonorUserId,
    decimal Amount,
    DateTime RequestedAt);
