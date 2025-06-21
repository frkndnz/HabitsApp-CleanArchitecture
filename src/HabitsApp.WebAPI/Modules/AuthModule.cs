using MediatR;
using HabitsApp.Application.Auth;
using HabitsApp.Domain.Shared;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
namespace HabitsApp.WebAPI.Modules;

public static class AuthModule
{
    public static void RegisterAuthRoutes(this RouteGroupBuilder routes)
    {
        RouteGroupBuilder routesGroup = routes.MapGroup("/auth").WithTags("Auth");
        

        routesGroup.MapPost("register", async (ISender sender, RegisterCommand request, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<Result<string>>();

        routesGroup.MapPost("login", async (HttpContext http, ISender sender, LoginCommand request, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
           
            if (response.IsSuccess)
            {
                http.Response.Cookies.Append("access_token", response.Value!.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                });
            }
            return response.IsSuccess ? Results.Ok(Result<AccountInfoDto>.Success(response.Value!.AccountInfo, "login success!")) : Results.BadRequest(response);
        })
            .Produces<Result<AccountInfoDto>>()
        .Produces<Result<LoginCommandResponse>>();


        routesGroup.MapPost("logout",  (HttpContext context) =>
        {
            context.Response.Cookies.Delete("access_token");
            var response = Result<string>.Success(null, "logout success!");
            return Results.Ok(response);
        }).Produces<Result<string>>()
        .RequireAuthorization();


        routesGroup.MapGet("info", async (ISender sender, CancellationToken cancellationToken) =>
        {
            AccountMeQuery query = new AccountMeQuery();
            var response = await sender.Send(query, cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
            .Produces<Result<AccountMeQueryResponse>>()
            .RequireAuthorization();



        routesGroup.MapGet("confirm-email", async (ISender sender, Guid userId, string token, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new ConfirmEmailCommand(userId, token), cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
         .Produces<Result<string>>();


        routesGroup.MapPost("google", async (HttpContext http, GoogleLoginCommand request, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);

            if (response.IsSuccess)
            {
                http.Response.Cookies.Append("access_token", response.Value!.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                });
            }
            return response.IsSuccess ? Results.Ok(Result<AccountInfoDto>.Success(response.Value!.AccountInfo, "login success!")) : Results.BadRequest(response);

        }).Produces<Result<AccountInfoDto>>()
            .Produces<Result<GoogleLoginCommandResponse>>();
    }
}
