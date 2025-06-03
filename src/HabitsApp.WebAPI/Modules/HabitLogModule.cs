using HabitsApp.Application.HabitLogs;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HabitsApp.WebAPI.Modules;

public static class HabitLogModule
{
    public static void RegisterHabitLogRoutes(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder routesGroup = routes.MapGroup("/habitlogs").WithTags("HabitLogs");

        routesGroup.MapPost(string.Empty, async (ISender sender, HabitLogCreateCommand request, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<Guid>>();

        routesGroup.MapDelete(string.Empty, async (Guid habitId, DateTime date, ISender sender, CancellationToken cancellation) =>
        {
            HabitLogDeleteCommand request = new(habitId, date);
            var response = await sender.Send(request, cancellation);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<string>>();
    }
}