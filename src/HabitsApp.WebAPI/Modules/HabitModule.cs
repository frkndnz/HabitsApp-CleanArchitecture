using HabitsApp.Application.Habits;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.WebAPI.Modules;

public static class HabitModule
{
    public static void RegisterHabitRoutes(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder routesGroup = routes.MapGroup("/habits").WithTags("Habits");

        routesGroup.MapPost(string.Empty, async (ISender sender, HabitCreateCommand request, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);

        })
        .Produces<Result<string>>();

        routesGroup.MapGet(string.Empty, async (ISender sender, HttpContext context, CancellationToken cancellationToken) =>
        {
            var userIdString = context.User.FindFirst("user_id")!.Value;
            Guid userId = Guid.Parse(userIdString);
            GetUserHabitsQuery request = new(userId);
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);

        }).Produces<Result<List<GetUserHabitsQueryResponse>>>();


        routesGroup.MapPut("/{id}", async (ISender sender,string id,HabitUpdateCommand request,CancellationToken cancellationToken) =>
        {
            request.Id = Guid.Parse(id);
            var response=await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<string>>();


        routesGroup.MapDelete("/{id}", async (ISender sender, string id, CancellationToken cancellationToken) =>
        {
            HabitDeleteCommand request = new(Guid.Parse(id));
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<string>>();
    }

}
