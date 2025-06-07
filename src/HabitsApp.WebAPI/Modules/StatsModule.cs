using System.Linq.Expressions;
using HabitsApp.Application.Stats;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.WebAPI.Modules;

public static class StatsModule
{
    public static void RegisterStatsRoutes(this IEndpointRouteBuilder builder)
    {
        var routesGroup = builder.MapGroup("/stats").WithTags("Stats");

        routesGroup.MapGet("summary", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var request = new GetSummaryStatsQuery();
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<GetSummaryStatsQueryResponse>>();
    }
}
