using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Auth;
public sealed record RegisterCommand(string UserName,string Email,string Password):IRequest<Result<string>>;


internal sealed class RegisterCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<RegisterCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        AppUser? existingUser=await userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName==request.UserName );

        if (existingUser is not null)
        {
            return Result<string>.Failure("Bu eposta veya kullanıcı adı ile zaten bir hesap olusturulmus!" );
        }

        AppUser newUser = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result= await userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Result<string>.Failure(errors);
        }

        return Result<string>.Success(string.Empty,"Kullanıcı başarıyla oluşturuldu!");  
    }
}

