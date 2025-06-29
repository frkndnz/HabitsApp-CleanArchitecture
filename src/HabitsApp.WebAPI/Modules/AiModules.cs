using HabitsApp.Application.Gemini;
using MediatR;

namespace HabitsApp.WebAPI.Modules;

public static class AiModules
{
    public static void RegisterAiRoutes(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder routes = builder.MapGroup("ai").WithTags("AiRoutes").RequireAuthorization();

        routes.MapPost("gemini", async (ISender sender,SendGeminiPromptCommand command,CancellationToken cancellationToken) =>
        {
            var response=await sender.Send(command, cancellationToken);
            return response.IsSuccess ? Results.Ok(response):Results.BadRequest(response);
        }).Produces<SendGeminiResponse>();
    }
}
