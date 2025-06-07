
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace HabitsApp.Application;
public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assm = typeof(ApplicationRegistrar).Assembly;
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assm);
            cfg.AddOpenBehavior(typeof(Behaviors.ValidationBehavior<,>));
            // validaton için behavior pipeline eklenecek!
        });
        services.AddAutoMapper(assm);
        services.AddValidatorsFromAssembly(assm);


        return services;
    }
}
