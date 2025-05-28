using MediatR;
using HabitsApp.Application.Auth;
using HabitsApp.Domain.Shared;
namespace HabitsApp.WebAPI.Modules;

public static class AuthModule
{
    public static void RegisterAuthRoutes(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder routesGroup = routes.MapGroup("/auth").WithTags("Auth");


        routesGroup.MapPost("register", async (ISender sender, RegisterCommand request, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<Result<string>>();

        routesGroup.MapPost("login",async(ISender sender,LoginCommand request,CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<Result<LoginCommandResponse>>();
    }
}
