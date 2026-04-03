namespace ONGES.Donate.Application.DTOs.Messages;

public sealed record UpdateCampaignDonationMessage(
    Guid CampaignId,
    decimal Amount,
    DateTime DonatedAt);
