using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HabitsApp.Infrastructure.Options;
public sealed class GoogleOptionsSetup : IPostConfigureOptions<GoogleOptions>
{
    private readonly IConfiguration _configuration;

    public GoogleOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void PostConfigure(string? name, GoogleOptions options)
    {
        options.ClientId = _configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = _configuration["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/auth/google/callback";
        options.SaveTokens = true;
        options.Scope.Add("email");
        options.Scope.Add("profile");
    }
}
