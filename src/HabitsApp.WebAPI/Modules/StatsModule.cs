using System.Linq.Expressions;
using HabitsApp.Application.Stats;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.WebAPI.Modules;

public static class StatsModule
{
    public static void RegisterStatsRoutes(this RouteGroupBuilder builder)
    {
        var routesGroup = builder.MapGroup("/stats").WithTags("Stats").RequireAuthorization();

        routesGroup.MapGet("summary", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var request = new GetSummaryStatsQuery();
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<GetSummaryStatsQueryResponse>>();

        routesGroup.MapGet("habit-details", async (ISender sender,CancellationToken cancellationToken) =>
        {
            var request = new GetHabitDetailsQuery();
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<GetHabitDetailsQueryResponse>>();

        routesGroup.MapGet("category-stats", async (ISender sender,CancellationToken cancellationToken) =>
        {
            GetCategoryStatsQuery query = new GetCategoryStatsQuery();
            var response=await sender.Send(query, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<List<GetCategoryStatsQueryResponse>>>();
    }
}
