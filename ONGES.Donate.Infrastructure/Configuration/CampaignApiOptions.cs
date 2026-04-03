namespace ONGES.Donate.Infrastructure.Configuration;

public sealed class CampaignApiOptions
{
    public const string SectionName = "CampaignApi";

    public string BaseUrl { get; set; } = string.Empty;
}
