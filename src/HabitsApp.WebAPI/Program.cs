using Google;
using HabitsApp.Application;
using HabitsApp.Infrastructure;
using HabitsApp.Infrastructure.Context;
using HabitsApp.WebAPI;
using HabitsApp.WebAPI.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);




builder.AddServiceDefaults();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog();

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();


builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer();

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromDays(1);
});
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminPolicy", policy =>
    policy.RequireRole("Admin"));
});
builder.Services.AddOpenApi();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}



app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseCors(opt =>
{

    opt.WithOrigins(app.Configuration["Cors:Origin"]!)
       .AllowAnyMethod()
       .AllowAnyHeader()
       .AllowCredentials();
});


app.RegisterRoutes();

app.MapControllers().RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
await ExtensionsMiddleware.CreateFirstUser(app);
app.UseExceptionHandler();
app.Run();
