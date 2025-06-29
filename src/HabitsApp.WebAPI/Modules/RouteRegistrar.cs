namespace HabitsApp.WebAPI.Modules;

public static class RouteRegistrar
{
    public static void RegisterRoutes(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api");

        apiGroup.RegisterAuthRoutes();
        apiGroup.RegisterHabitRoutes();
        apiGroup.RegisterHabitLogRoutes();
        apiGroup.RegisterStatsRoutes();
        apiGroup.RegisterCategoriesRoutes();
        apiGroup.RegisterUsersRoutes();
        apiGroup.RegisterBlogsRoutes();
        apiGroup.RegisterFeedbackRoutes();
        apiGroup.AdminRegisterRoutes();

        apiGroup.RegisterAiRoutes();


        RouteGroupBuilder testroutes = apiGroup.MapGroup("test").WithTags("test");
        testroutes.MapGet(string.Empty, () =>
        {
            throw new Exception("test amaçlı hata!");
        });
    }
}
