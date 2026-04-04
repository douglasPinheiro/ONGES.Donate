using ONGES.Donate.Application.DTOs.Responses;
using ONGES.Donate.Application.Interfaces;

namespace ONGES.Donate.Api.Endpoints.Donations;

public sealed class GetAll : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Listar Doacoes")
            .WithSummary("Lista todas as doacoes.")
            .WithDescription("Retorna todos os dados de doacao sem exigir autenticacao.")
            .Produces<List<DonationResponse>>(200)
            .AllowAnonymous();

    private static async Task<IResult> HandleAsync(
        IDonationService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result);
    }
}
