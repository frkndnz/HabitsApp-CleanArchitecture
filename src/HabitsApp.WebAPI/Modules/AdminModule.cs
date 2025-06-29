using HabitsApp.Application.AdminDashboard;
using HabitsApp.Application.Logs.Dtos;
using HabitsApp.Application.SeriLogs;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.WebAPI.Modules;

public static class AdminModule
{
    public static void AdminRegisterRoutes(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder routes = builder.MapGroup("admin").WithTags("Admin").RequireAuthorization("AdminPolicy");


        routes.MapGet("dashboard", async (ISender sender, CancellationToken cancellationToken) =>
        {
            GetAdminDashboardQuery request = new GetAdminDashboardQuery();
            var response = await sender.Send(request, cancellationToken);

            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        })
            .Produces<Result<AdminDashboardQueryResponse>>();

        routes.MapGet("logs/summary",async(ISender sender,CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetLogSummaryStatsQuery(), cancellationToken);
            return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        }) .Produces<Result<LogSummaryStatsDto>>();

        routes.MapGet("logs", async ([AsParameters]GetLogsQuery query,ISender sender,CancellationToken cancellationToken) =>
        {
            var response=await sender.Send(query, cancellationToken);
            return response.IsSuccess ? Results.Ok(response): Results.BadRequest(response);
        }).Produces<Result<GetLogsQueryResponse>>();
    }
            
        
}
