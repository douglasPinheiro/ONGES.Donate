using Moq;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Application.Services;
using ONGES.Contracts.DTOs;

namespace ONGES.Donate.Test.Application;

public sealed class DonationMessageProcessorTests
{
    [Fact]
    public async Task ProcessAsync_ShouldReturnSuccessWithoutPersisting_WhenDonationAlreadyExists()
    {
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<ICampaignUpdatePublisher>();

        repository
            .Setup(current => current.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var processor = new DonationMessageProcessor(repository.Object, publisher.Object);

        var result = await processor.ProcessAsync(
            new DonationRequestedMessage(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 25, DateTime.UtcNow),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Verify(current => current.AddAsync(It.IsAny<ONGES.Donate.Domain.Entities.DonationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(current => current.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        publisher.Verify(current => current.PublishAsync(It.IsAny<DonationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_ShouldPersistAndPublish_WhenDonationIsNew()
    {
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<ICampaignUpdatePublisher>();

        var donationId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var donorId = Guid.NewGuid();
        var requestedAt = DateTime.UtcNow.AddMinutes(-5);

        repository
            .Setup(current => current.ExistsAsync(donationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var processor = new DonationMessageProcessor(repository.Object, publisher.Object);

        var result = await processor.ProcessAsync(
            new DonationRequestedMessage(donationId, campaignId, donorId, 125, requestedAt),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        repository.Verify(
            current => current.AddAsync(
                It.Is<ONGES.Donate.Domain.Entities.DonationEntity>(donation =>
                    donation.Id == donationId &&
                    donation.CampaignId == campaignId &&
                    donation.DonorUserId == donorId &&
                    donation.Amount == 125 &&
                    donation.Status == ONGES.Donate.Domain.Enums.DonationStatus.Processed),
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(current => current.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        publisher.Verify(
            current => current.PublishAsync(
                It.Is<DonationMessage>(message =>
                    message.CampaignId == campaignId &&
                    message.Amount == 125),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
