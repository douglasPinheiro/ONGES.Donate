using FluentValidation;
using ONGES.Donate.Application.DTOs.Messages;
using ONGES.Donate.Application.DTOs.Requests;
using ONGES.Donate.Application.DTOs.Responses;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Domain.Shared;

namespace ONGES.Donate.Application.Services;

public sealed class DonationService(
    ICampaignValidationGateway campaignValidationGateway,
    IInternalDonationMessagePublisher internalDonationMessagePublisher,
    IValidator<CreateDonationRequest> validator) : IDonationService
{
    public async Task<Result<CreateDonationResponse>> RequestDonationAsync(
        CreateDonationRequest request,
        Guid donorUserId,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return Result<CreateDonationResponse>.Failure(new Error(
                "400",
                string.Join(", ", validation.Errors.Select(error => error.ErrorMessage))));
        }

        if (donorUserId == Guid.Empty)
            return Result<CreateDonationResponse>.Failure(new Error("401", "Usuario autenticado invalido."));

        var campaignExists = await campaignValidationGateway.CampaignExistsAsync(request.IdCampanha, cancellationToken);

        if (!campaignExists)
            return Result<CreateDonationResponse>.Failure(new Error("404", "A campanha informada nao foi encontrada."));

        var isCampaignActive = await campaignValidationGateway.IsCampaignActiveAsync(request.IdCampanha, cancellationToken);

        if (!isCampaignActive)
            return Result<CreateDonationResponse>.Failure(new Error("400", "A campanha informada nao esta ativa."));

        var donationId = Guid.NewGuid();
        var requestedAt = DateTime.UtcNow;

        await internalDonationMessagePublisher.PublishAsync(
            new DonationRequestedMessage(donationId, request.IdCampanha, donorUserId, request.ValorDoado, requestedAt),
            cancellationToken);

        return Result<CreateDonationResponse>.Success(new CreateDonationResponse(
            donationId,
            request.IdCampanha,
            request.ValorDoado,
            requestedAt,
            "Pending"));
    }
}
