using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HabitsApp.Application.Auth;
public sealed record ConfirmEmailCommand
(
    Guid UserId,
    string Token
    ) : IRequest<Result<string>>;


internal sealed class ConfirmEmailCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<ConfirmEmailCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result<string>.Failure("Not found user!");
        }


        var result = await userManager.ConfirmEmailAsync(user, request.Token);


        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Description.Contains("Invalid token")))
            {
                return Result<string>.Failure("InvalidToken");
            }
            return Result<string>.Failure("Email confirmation failed!");

        }
        return Result<string>.Success(null, "Email confirmed");
    }
}
