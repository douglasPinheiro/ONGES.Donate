using ONGES.Donate.Application.DTOs.Requests;
using ONGES.Donate.Application.DTOs.Responses;
using ONGES.Donate.Application.Interfaces;
using System.Security.Claims;

namespace ONGES.Donate.Api.Endpoints.Donations;

public sealed class Create : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar Doacao")
            .WithSummary("Recebe uma solicitacao de doacao.")
            .WithDescription("Recebe a doacao de um usuario autenticado e envia para processamento assincrono.")
            .Produces<CreateDonationResponse>(202)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization();

    private static async Task<IResult> HandleAsync(
        HttpContext context,
        CreateDonationRequest request,
        IDonationService service,
        CancellationToken cancellationToken)
    {
        var userIdClaim = context.User.FindFirstValue("UserId");

        if (!Guid.TryParse(userIdClaim, out var donorUserId))
            return Results.Unauthorized();

        var result = await service.RequestDonationAsync(request, donorUserId, cancellationToken);

        return result.IsSuccess
            ? Results.Accepted($"/v1/donations/{result.Value!.DonationId}", result.Value)
            : Results.BadRequest(result);
    }
}
