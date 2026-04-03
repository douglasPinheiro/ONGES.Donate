using ONGES.Donate.Domain.Entities;
using ONGES.Donate.Domain.Enums;
using ONGES.Donate.Domain.Exceptions;

namespace ONGES.Donate.Test.Domain;

public sealed class DonationEntityTests
{
    [Fact]
    public void Create_ShouldThrow_WhenAmountIsInvalid()
    {
        var action = () => DonationEntity.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            DateTime.UtcNow);

        Assert.Throws<DomainValidationException>(action);
    }

    [Fact]
    public void MarkAsProcessed_ShouldUpdateStatus()
    {
        var donation = DonationEntity.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            DateTime.UtcNow);

        donation.MarkAsProcessed(DateTime.UtcNow);

        Assert.Equal(DonationStatus.Processed, donation.Status);
        Assert.NotNull(donation.ProcessedAt);
    }
}
