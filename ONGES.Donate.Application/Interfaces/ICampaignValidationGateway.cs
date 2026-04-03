namespace ONGES.Donate.Application.Interfaces;

public interface ICampaignValidationGateway
{
    Task<bool> CampaignExistsAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<bool> IsCampaignActiveAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
