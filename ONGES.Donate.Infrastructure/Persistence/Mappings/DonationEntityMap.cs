using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONGES.Donate.Domain.Entities;

namespace ONGES.Donate.Infrastructure.Persistence.Mappings;

public sealed class DonationEntityMap : IEntityTypeConfiguration<DonationEntity>
{
    public void Configure(EntityTypeBuilder<DonationEntity> builder)
    {
        builder.ToTable("Donations");

        builder.HasKey(donation => donation.Id);

        builder.Property(donation => donation.Amount)
            .HasPrecision(18, 2);

        builder.Property(donation => donation.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(donation => donation.RequestedAt).IsRequired();
        builder.Property(donation => donation.CreatedAt).IsRequired();

        builder.HasIndex(donation => donation.CampaignId);
    }
}
