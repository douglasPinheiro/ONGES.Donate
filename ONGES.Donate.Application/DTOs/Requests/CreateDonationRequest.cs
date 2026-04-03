namespace ONGES.Donate.Application.DTOs.Requests;

public sealed record CreateDonationRequest(Guid IdCampanha, decimal ValorDoado);
