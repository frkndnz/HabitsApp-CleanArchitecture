using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using System.Web;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Auth;
public sealed record ResendConfirmationCommand(
    string userId):IRequest<Result<string>>;



internal sealed class ResendConfirmationCommandHandler(UserManager<AppUser> userManager,IConfiguration configuration,IEmailService emailService) : IRequestHandler<ResendConfirmationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ResendConfirmationCommand request, CancellationToken cancellationToken)
    {
        Guid userId=Guid.Parse(request.userId);
        var user = await userManager.Users.FirstOrDefaultAsync(u=>u.Id== userId);
        if (user == null || user.EmailConfirmed)
            return Result<string>.Failure("User not found or already confirmed");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user); // confirm token
        var encodedToken = HttpUtility.UrlEncode(token); // token url encode

        var url = configuration["Cors:Origin"];
        await emailService.SendEmailAsync(user.Email!, "HabitsApp Hesap Onayı",
            $"Hesabınızı onaylamak için lütfen <a href='{url}/auth/confirm-email?userId={user.Id}&token={encodedToken}'>buraya</a> tıklayın.");

        return Result<string>.Success(null, "Confirmation email resent!");
    }
}

