﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Logs;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.Blogs;
using HabitsApp.Domain.Categories;
using HabitsApp.Domain.Feedbacks;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Users;
using HabitsApp.Infrastructure.Context;
using HabitsApp.Infrastructure.Logging;
using HabitsApp.Infrastructure.Options;
using HabitsApp.Infrastructure.Repositories;
using HabitsApp.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HabitsApp.Infrastructure;
public static class InfrastructureRegistrar
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration,IWebHostEnvironment environment)
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

        SerilogSetup.ConfigureSerilog(configuration);

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.ConfigureOptions<JwtOptionsSetup>();
        
       

        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IJwtProvider,JwtProvider>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<IHabitLogRepository, HabitLogRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBlogPostRepository,BlogPostRepository>();
        services.AddScoped<IFeedbackRepository,FeedbackRepository>();
        services.AddScoped<ILogRepository, LogRepository>();    

        services.AddScoped<ICurrentUserService,CurrentUserService>();
        services.AddScoped<IGoogleAuthValidator,GoogleAuthValidator>();

        services.AddScoped<IGeminiService, GeminiService>();

        Console.WriteLine($"Environment: {environment.EnvironmentName}");
        if (environment.IsDevelopment())
        {
            services.AddScoped<IFileStorage, FileStorage>();
        }
        else
        {
            services.AddScoped<IFileStorage,BlobStorageService>();
        }
        
        services.AddScoped<IUrlService, UrlService>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddHttpContextAccessor();
    }
}
