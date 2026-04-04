using FluentValidation;
using Moq;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.DTOs.Requests;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Application.Services;
using ONGES.Donate.Domain.Entities;

namespace ONGES.Donate.Test.Application;

public sealed class DonationServiceTests
{
    [Fact]
    public async Task RequestDonationAsync_ShouldFail_WhenRequestIsInvalid()
    {
        var campaignGateway = new Mock<ICampaignValidationGateway>();
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<IInternalDonationMessagePublisher>();
        var validator = new InlineValidator<CreateDonationRequest>();
        validator.RuleFor(request => request.IdCampanha).NotEmpty();
        validator.RuleFor(request => request.ValorDoado).GreaterThan(0);

        var service = new DonationService(campaignGateway.Object, repository.Object, publisher.Object, validator);

        var result = await service.RequestDonationAsync(
            new CreateDonationRequest(Guid.Empty, 0),
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("400", result.Error?.Code);
        campaignGateway.Verify(
            gateway => gateway.CampaignExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        publisher.Verify(
            current => current.PublishAsync(It.IsAny<DonationRequestedMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RequestDonationAsync_ShouldFail_WhenUserIsInvalid()
    {
        var campaignGateway = new Mock<ICampaignValidationGateway>();
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<IInternalDonationMessagePublisher>();
        var validator = new InlineValidator<CreateDonationRequest>();
        validator.RuleFor(request => request.IdCampanha).NotEmpty();
        validator.RuleFor(request => request.ValorDoado).GreaterThan(0);

        var service = new DonationService(campaignGateway.Object, repository.Object, publisher.Object, validator);

        var result = await service.RequestDonationAsync(
            new CreateDonationRequest(Guid.NewGuid(), 10),
            Guid.Empty,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("401", result.Error?.Code);
        campaignGateway.Verify(
            gateway => gateway.CampaignExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        publisher.Verify(
            current => current.PublishAsync(It.IsAny<DonationRequestedMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RequestDonationAsync_ShouldFail_WhenCampaignDoesNotExist()
    {
        var campaignGateway = new Mock<ICampaignValidationGateway>();
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<IInternalDonationMessagePublisher>();
        var validator = new InlineValidator<CreateDonationRequest>();
        validator.RuleFor(request => request.IdCampanha).NotEmpty();
        validator.RuleFor(request => request.ValorDoado).GreaterThan(0);

        campaignGateway
            .Setup(gateway => gateway.CampaignExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new DonationService(campaignGateway.Object, repository.Object, publisher.Object, validator);

        var result = await service.RequestDonationAsync(
            new CreateDonationRequest(Guid.NewGuid(), 25),
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("404", result.Error?.Code);
        campaignGateway.Verify(
            gateway => gateway.IsCampaignActiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        publisher.Verify(
            current => current.PublishAsync(It.IsAny<DonationRequestedMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedDonations()
    {
        var campaignGateway = new Mock<ICampaignValidationGateway>();
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<IInternalDonationMessagePublisher>();
        var validator = new InlineValidator<CreateDonationRequest>();

        var first = ONGES.Donate.Domain.Entities.DonationEntity.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            DateTime.UtcNow.AddHours(-2));

        first.MarkAsProcessed(DateTime.UtcNow.AddHours(-1));

        var second = ONGES.Donate.Domain.Entities.DonationEntity.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            50,
            DateTime.UtcNow);

        repository
            .Setup(current => current.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([first, second]);

        var service = new DonationService(campaignGateway.Object, repository.Object, publisher.Object, validator);

        var result = await service.GetAllAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        var donations = Assert.IsAssignableFrom<IEnumerable<ONGES.Donate.Application.DTOs.Responses.DonationResponse>>(result.Value);
        Assert.Equal(2, donations.Count());
        Assert.Contains(donations, donation => donation.Id == first.Id && donation.Status == "Processed");
        Assert.Contains(donations, donation => donation.Id == second.Id && donation.Status == "Pending");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenThereAreNoDonations()
    {
        var campaignGateway = new Mock<ICampaignValidationGateway>();
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<IInternalDonationMessagePublisher>();
        var validator = new InlineValidator<CreateDonationRequest>();

        repository
            .Setup(current => current.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var service = new DonationService(campaignGateway.Object, repository.Object, publisher.Object, validator);

        var result = await service.GetAllAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task RequestDonationAsync_ShouldFail_WhenCampaignIsInactive()
    {
        var campaignGateway = new Mock<ICampaignValidationGateway>();
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<IInternalDonationMessagePublisher>();
        var validator = new InlineValidator<CreateDonationRequest>();
        validator.RuleFor(request => request.IdCampanha).NotEmpty();
        validator.RuleFor(request => request.ValorDoado).GreaterThan(0);

        campaignGateway
            .Setup(gateway => gateway.CampaignExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        campaignGateway
            .Setup(gateway => gateway.IsCampaignActiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new DonationService(campaignGateway.Object, repository.Object, publisher.Object, validator);

        var result = await service.RequestDonationAsync(
            new CreateDonationRequest(Guid.NewGuid(), 50),
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        publisher.Verify(
            current => current.PublishAsync(It.IsAny<DonationRequestedMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RequestDonationAsync_ShouldPublishMessage_WhenCampaignExistsAndIsActive()
    {
        var campaignGateway = new Mock<ICampaignValidationGateway>();
        var repository = new Mock<IDonationRepository>();
        var publisher = new Mock<IInternalDonationMessagePublisher>();
        var validator = new InlineValidator<CreateDonationRequest>();
        validator.RuleFor(request => request.IdCampanha).NotEmpty();
        validator.RuleFor(request => request.ValorDoado).GreaterThan(0);

        var campaignId = Guid.NewGuid();
        var donorId = Guid.NewGuid();

        campaignGateway
            .Setup(gateway => gateway.CampaignExistsAsync(campaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        campaignGateway
            .Setup(gateway => gateway.IsCampaignActiveAsync(campaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new DonationService(campaignGateway.Object, repository.Object, publisher.Object, validator);

        var result = await service.RequestDonationAsync(
            new CreateDonationRequest(campaignId, 80),
            donorId,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(campaignId, result.Value!.CampaignId);
        Assert.Equal(80, result.Value.Amount);
        Assert.Equal("Pending", result.Value.Status);

        publisher.Verify(
            current => current.PublishAsync(
                It.Is<DonationRequestedMessage>(message =>
                    message.CampaignId == campaignId &&
                    message.DonorUserId == donorId &&
                    message.Amount == 80),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
