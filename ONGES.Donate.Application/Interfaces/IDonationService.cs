using ONGES.Donate.Application.DTOs.Requests;
using ONGES.Donate.Application.DTOs.Responses;
using ONGES.Donate.Domain.Shared;

namespace ONGES.Donate.Application.Interfaces;

public interface IDonationService
{
    Task<Result<IEnumerable<DonationResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<CreateDonationResponse>> RequestDonationAsync(
        CreateDonationRequest request,
        Guid donorUserId,
        CancellationToken cancellationToken = default);
}
