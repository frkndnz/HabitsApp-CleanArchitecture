using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Auth;
public sealed  record GoogleLoginCommand
    (
     string IdToken 
    ):IRequest<Result<GoogleLoginCommandResponse>>;

public class GoogleLoginCommandResponse
{
    public string AccessToken { get; set; } = default!;
}

internal sealed class GoogleLoginCommandHandler
    (
        UserManager<AppUser> userManager, 
        IJwtProvider jwtProvider,
        IGoogleAuthValidator googleAuthValidator
    ) : IRequestHandler<GoogleLoginCommand, Result<GoogleLoginCommandResponse>>
{
    public async Task<Result<GoogleLoginCommandResponse>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var googlePayload=await googleAuthValidator.ValidateGoogleIdTokenAsync( request.IdToken );
        if (googlePayload == null)
            return Result<GoogleLoginCommandResponse>.Failure("geçersiz google Id token");
         

        var user = await userManager.FindByLoginAsync("Google", googlePayload.Subject);

        if ( user == null )
        {
            user = await userManager.FindByEmailAsync(googlePayload.Email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = googlePayload.Email,
                    Email = googlePayload.Email,
                    FirstName = googlePayload.GivenName,
                    LastName = googlePayload.FamilyName,
                    EmailConfirmed = true, // Google ile doğrulandığı için onaylı yapabilirsin
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return Result<GoogleLoginCommandResponse>.Failure("user creation failed!");
            }
            
        }

        var userLogins = await userManager.GetLoginsAsync(user);
        var googleLoginInfo = new UserLoginInfo("Google", googlePayload.Subject, "Google");

        if (!userLogins.Any(ul => ul.LoginProvider == googleLoginInfo.LoginProvider && ul.ProviderKey == googleLoginInfo.ProviderKey))
        {
            var addLoginResult = await userManager.AddLoginAsync(user, googleLoginInfo);
            if (!addLoginResult.Succeeded)
                return Result<GoogleLoginCommandResponse>.Failure("Google hesabı kullanıcıya bağlanamadı.");
        }


        var token = await jwtProvider.CreateTokenAsync(user);
        GoogleLoginCommandResponse response = new()
        {
            AccessToken = token,
        };
        return Result<GoogleLoginCommandResponse>.Success(response, "success");

    }
}
