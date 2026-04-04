namespace ONGES.Donate.Application.DTOs.Responses;

public sealed record DonationResponse(
    Guid Id,
    Guid CampaignId,
    Guid DonorUserId,
    decimal Amount,
    string Status,
    DateTime RequestedAt,
    DateTime? ProcessedAt,
    DateTime CreatedAt);
