using HabitsApp.Application.Feedbacks;

using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.WebAPI.Modules;

public static class FeedbackModule
{
    public static void  RegisterFeedbackRoutes(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder routes = builder.MapGroup("feedbacks").WithTags("Feedbacks");

        routes.MapPost(string.Empty, async (ISender sender, CreateFeedbackCommand request, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }).Produces<Result<string>>().RequireAuthorization();


        routes.MapGet(string.Empty, async (ISender sender, int Page, int PageSize, CancellationToken cancellationToken) =>
        {
            var request = new GetAllFeedbackQuery(Page, PageSize);
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
           .Produces<GetAllFeedbackResponse>()
           .RequireAuthorization("AdminPolicy");
    }
}
