using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ONGES.Donate.Application.Interfaces;

namespace ONGES.Donate.Infrastructure.Services;

public sealed class CampaignValidationGateway(HttpClient httpClient) : ICampaignValidationGateway
{
    public async Task<bool> CampaignExistsAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"/v1/campaigns/{campaignId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> IsCampaignActiveAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"/v1/campaigns/{campaignId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return false;

        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>(cancellationToken: cancellationToken);

        return string.Equals(campaign?.Status, "Active", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record CampaignResponse([property: JsonPropertyName("status")] string Status);
}
