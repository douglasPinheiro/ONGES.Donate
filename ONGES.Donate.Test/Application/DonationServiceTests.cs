using FluentValidation;
using Moq;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.DTOs.Requests;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Application.Services;

namespace ONGES.Donate.Test.Application;

public sealed class DonationServiceTests
{
    [Fact]
    public async Task RequestDonationAsync_ShouldFail_WhenCampaignIsInactive()
    {
        var campaignGateway = new Mock<ICampaignValidationGateway>();
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

        var service = new DonationService(campaignGateway.Object, publisher.Object, validator);

        var result = await service.RequestDonationAsync(
            new CreateDonationRequest(Guid.NewGuid(), 50),
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        publisher.Verify(
            current => current.PublishAsync(It.IsAny<DonationRequestedMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
