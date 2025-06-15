namespace HabitsApp.WebAPI.Modules;

public static class RouteRegistrar
{
    public static void RegisterRoutes(this IEndpointRouteBuilder app)
    {
        app.RegisterAuthRoutes();
        app.RegisterHabitRoutes();
        app.RegisterHabitLogRoutes();
        app.RegisterStatsRoutes();
        app.RegisterCategoriesRoutes();
        app.RegisterUsersRoutes();
        app.RegisterBlogsRoutes();
    }
}
