using ONGES.Donate.Domain.Enums;
using ONGES.Donate.Domain.Exceptions;

namespace ONGES.Donate.Domain.Entities;

public sealed class DonationEntity
{
    private DonationEntity()
    {
    }

    private DonationEntity(Guid id, Guid campaignId, Guid donorUserId, decimal amount, DateTime requestedAt)
    {
        Id = id;
        CampaignId = campaignId;
        DonorUserId = donorUserId;
        Amount = amount;
        RequestedAt = requestedAt;
        Status = DonationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CampaignId { get; private set; }
    public Guid DonorUserId { get; private set; }
    public decimal Amount { get; private set; }
    public DonationStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static DonationEntity Create(Guid id, Guid campaignId, Guid donorUserId, decimal amount, DateTime requestedAt)
    {
        if (id == Guid.Empty)
            throw new DomainValidationException("O id da doacao e obrigatorio.");

        if (campaignId == Guid.Empty)
            throw new DomainValidationException("O id da campanha e obrigatorio.");

        if (donorUserId == Guid.Empty)
            throw new DomainValidationException("O id do doador e obrigatorio.");

        if (amount <= 0)
            throw new DomainValidationException("O valor da doacao deve ser maior que zero.");

        return new DonationEntity(id, campaignId, donorUserId, amount, requestedAt);
    }

    public void MarkAsProcessed(DateTime processedAt)
    {
        Status = DonationStatus.Processed;
        ProcessedAt = processedAt;
    }
}
