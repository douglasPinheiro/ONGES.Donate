using MassTransit;

namespace ONGES.Donate.Application.DTOs.Messages;

[EntityName("update-campaign-donation")]
public sealed record UpdateCampaignDonationMessage(
    Guid CampaignId,
    decimal Amount,
    DateTime DonatedAt);
