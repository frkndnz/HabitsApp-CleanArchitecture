using HabitsApp.Application.Categories;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HabitsApp.WebAPI.Modules;

public static class CategoryModule
{
    public static void RegisterCategoriesRoutes(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder routes = builder.MapGroup("categories").WithTags("Categories").RequireAuthorization();

        routes.MapPost(string.Empty, async (ISender sender, CreateCategoryCommand request, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<CreateCategoryCommandResponse>>()
        ;


        routes.MapGet(string.Empty, async (ISender sender,CancellationToken cancellationToken) =>
        {
            GetCategoriesQuery query = new GetCategoriesQuery();
            var response=await sender.Send(query, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<List<GetCategoriesQueryResponse>>>();
    }
}
