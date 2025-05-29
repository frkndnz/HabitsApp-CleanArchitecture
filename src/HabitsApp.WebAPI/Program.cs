using HabitsApp.Application;
using HabitsApp.Infrastructure;
using HabitsApp.WebAPI;
using HabitsApp.WebAPI.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();


builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseCors(opt =>
{
    opt.AllowAnyMethod();
    opt.AllowAnyHeader();
    opt.AllowCredentials();
    opt.SetIsOriginAllowed(origin => true);
});


app.RegisterRoutes();
app.MapGet("/", () => "Hello World!").RequireAuthorization();
app.MapControllers().RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

await ExtensionsMiddleware.CreateFirstUser(app);
app.UseExceptionHandler();
app.Run();
