using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Blogs;
using HabitsApp.Domain.Categories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Users;
using HabitsApp.Infrastructure.Context;
using HabitsApp.Infrastructure.Options;
using HabitsApp.Infrastructure.Repositories;
using HabitsApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HabitsApp.Infrastructure;
public static class InfrastructureRegistrar
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("SqlServer")!;
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(connectionString);
        });

        services.AddIdentity<AppUser, IdentityRole<Guid>>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequiredLength = 8;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;

            opt.Lockout.MaxFailedAccessAttempts = 5;
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

            opt.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.ConfigureOptions<JwtOptionsSetup>();
        


        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IJwtProvider,JwtProvider>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<IHabitLogRepository, HabitLogRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBlogPostRepository,BlogPostRepository>();
        services.AddScoped<ICurrentUserService,CurrentUserService>();
        services.AddScoped<IGoogleAuthValidator,GoogleAuthValidator>();

        services.AddScoped<IFileStorage,FileStorage>();
        services.AddScoped<IUrlService, UrlService>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddHttpContextAccessor();
    }
}
