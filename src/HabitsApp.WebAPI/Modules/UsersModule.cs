using HabitsApp.Application.Users;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.WebAPI.Modules;

public static class UsersModule
{
    public static void RegisterUsersRoutes(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder routesGroup = builder.MapGroup("/users").WithTags("Users");

        routesGroup.MapGet(string.Empty, async (ISender sender,string? SearchTerm, int Page, int PageSize, CancellationToken cancellationToken) =>
        {
            var request = new GetUsersAllQuery(Page, PageSize,SearchTerm);
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
            .Produces<GetUsersAllQueryResponse>()
            .RequireAuthorization("AdminPolicy");


        routesGroup.MapGet("profile",async (ISender sender,CancellationToken cancellationToken) =>
        {
            GetUserProfileQuery request = new GetUserProfileQuery();
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }) .Produces<GetUserProfileQueryResponse>()
        .RequireAuthorization();


        routesGroup.MapPut("profile/{id}", async (ISender sender, string id, UpdateUserProfileCommand request, CancellationToken cancellationToken) =>
        {
            if (request.Id != Guid.Parse(id))
            {
                var result = Result<UpdateUserProfileCommand>.Failure("id not match!");
                return Results.BadRequest(result);
            }

            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<string>>()
        .RequireAuthorization();


        routesGroup.MapPut("/{id}", async (ISender sender, string id, UpdateUserCommand request , CancellationToken cancellationToken) =>
        {
            if (request.Id != Guid.Parse(id))
            {
                var result = Result<UpdateUserCommandResponse>.Failure("id not match!");
                return Results.BadRequest(result);
            }

            var response=await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response); 
        })
            .Produces<UpdateUserCommandResponse>()
            .RequireAuthorization("AdminPolicy"); 
    }
}
