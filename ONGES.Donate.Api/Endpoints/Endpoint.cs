using ONGES.Donate.Api.Endpoints.Donations;

namespace ONGES.Donate.Api.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup(string.Empty);

        endpoints.MapGroup("v1/donations")
            .WithTags("Doacoes")
            .MapEndpoint<Create>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
