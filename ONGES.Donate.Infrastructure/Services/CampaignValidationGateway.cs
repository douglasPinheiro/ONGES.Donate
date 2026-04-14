using ONGES.Donate.Application.DTOs.Responses;
using ONGES.Donate.Application.Interfaces;
using System.Net.Http.Json;

namespace ONGES.Donate.Infrastructure.Services;

public sealed class CampaignValidationGateway(HttpClient httpClient) : ICampaignValidationGateway
{
    public async Task<bool> CampaignExistsAsync(Guid campaignId, CancellationToken ct = default)
    {
        using var response = await httpClient.GetAsync($"/api/v1/campaigns/internal/{campaignId}", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> IsCampaignActiveAsync(Guid campaignId, CancellationToken ct = default)
    {
        using var response = await httpClient.GetAsync($"/api/v1/campaigns/internal/{campaignId}", ct);

        if (!response.IsSuccessStatusCode)
            return false;

        var campaign = await response.Content.ReadFromJsonAsync<CampaignValidationResponse>(ct);
        return campaign?.Status == "Active";
    }
}
