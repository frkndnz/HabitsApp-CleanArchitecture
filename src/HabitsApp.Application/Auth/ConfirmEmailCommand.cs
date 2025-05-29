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
    ):IRequest<Result<string>>;


internal sealed class ConfirmEmailCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<ConfirmEmailCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user=await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result<string>.Failure("Kullanıcı bulunamadı!");
        }

        
        var result = await userManager.ConfirmEmailAsync(user, request.Token);


        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure($"Eposta adresiniz onaylanamadı: {errors}");
           
        }
        return Result<string>.Success(null,"Eposta adresiniz başarıyla onaylandı!");
    }
}
