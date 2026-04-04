namespace ONGES.Donate.Infrastructure.Configuration;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public string ConnectionString { get; set; } = string.Empty;
    public string DonationsTopic { get; set; } = "donates-topic";
    public string DonationsSubscription { get; set; } = "donate-api-sub";
    public string CampaignUpdatesEntity { get; set; } = "campaigns-topic";
}
