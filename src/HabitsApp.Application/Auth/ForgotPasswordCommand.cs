using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace HabitsApp.Application.Auth;
public sealed record ForgotPasswordCommand(
    string Email
    ):IRequest<Result<string>>;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty!")
            .EmailAddress().WithMessage("please enter valid email address!");
    }
}
internal sealed class ForgotPasswordCommandHandler(
    UserManager<AppUser> userManager,
    IEmailService emailService,
    IConfiguration configuration
    ) : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user=await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<string>.Failure("not found user by email");
        }
        if(!await userManager.IsEmailConfirmedAsync(user))
        {

            return Result<string>.Failure("email not confirmed");
        }

        var token=await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken=WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var url = configuration["Cors:Origin"];

        var resetLink = $"{url}/auth/reset-password?email={user.Email}&token={encodedToken}";

        await emailService.SendEmailAsync(user.Email!, "Reset your password", $"Click <a href='{resetLink}'>here</a> to reset your password.");

        return Result<string>.Success("success", "successfully send email!");
    }
}

