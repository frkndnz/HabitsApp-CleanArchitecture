using HabitsApp.Application.BlogPosts;
using HabitsApp.Application.Blogs;
using HabitsApp.Application.Habits;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HabitsApp.WebAPI.Modules;

public static class BlogPostModule
{
    public static void RegisterBlogsRoutes(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder routes = builder.MapGroup("/blogs").WithTags("Blogs");


        routes.MapGet(string.Empty, async (ISender sender, CancellationToken cancellationToken) =>
        {
            GetAllBlogPostsQuery request = new GetAllBlogPostsQuery();
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<List<GetAllBlogPostsQueryResponse>>>();

        routes.MapGet("/{id}",async (ISender sender,string id,CancellationToken cancellationToken) =>
        {
            var blogId=Guid.Parse(id);
            BlogPostGetByIdQuery request = new(blogId);
            var response=await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<BlogPostGetByIdQueryResponse>>();

        routes.MapPost(string.Empty, async ([FromForm] BlogPostCreateCommand request, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
            .DisableAntiforgery()
            .Accepts<BlogPostCreateCommand>("multipart/form-data")
            .Produces<Result<Guid>>();

        routes.MapPut("/{id}", async (ISender sender, string id, [FromForm] BlogPostUpdateCommand request, CancellationToken cancellationToken) =>
        {
            request.Id=Guid.Parse(id);  
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).DisableAntiforgery()
            .Accepts<BlogPostUpdateCommand>("multipart/form-data")
            .Produces<Result<Guid>>();

        routes.MapDelete("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            BlogPostDeleteCommand request = new BlogPostDeleteCommand(id);
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<Guid>>();
    }
}
