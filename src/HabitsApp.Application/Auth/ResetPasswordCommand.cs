using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace HabitsApp.Application.Auth;
public sealed record ResetPasswordCommand(
    string Token,
    string Email,
    string NewPassword
    ):IRequest<Result<string>>;


public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Password cannot be blank!");
        RuleFor(x => x.NewPassword).MinimumLength(8).WithMessage("Password must be at least 8 characters!");
    }
}

internal sealed class ResetPasswordCommandHandler(
    UserManager<AppUser> userManager
    ) : IRequestHandler<ResetPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user =await userManager.FindByEmailAsync(request.Email);
        if(user == null)
        {
            return Result<string>.Failure("Invalid email!");
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result=await userManager.ResetPasswordAsync(user,decodedToken,request.NewPassword);

        if (result.Succeeded)
            return Result<string>.Success(null, "password reset successfully!");

        return Result<string>.Failure(result.Errors.Select(e => e.Description).ToArray());
    }
}

