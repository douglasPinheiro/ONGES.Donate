namespace ONGES.Donate.Application.DTOs.Responses;

public sealed record CreateDonationResponse(
    Guid DonationId,
    Guid CampaignId,
    decimal Amount,
    DateTime RequestedAt,
    string Status);
