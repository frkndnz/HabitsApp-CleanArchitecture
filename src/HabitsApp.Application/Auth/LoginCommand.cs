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
public sealed record LoginCommand
(
    string EmailOrUserName,
    string Password
    ) : IRequest<Result<LoginCommandResponse>>;


public sealed record LoginCommandResponse
{
    public string AccessToken { get; set; } = default!;
    public required AccountInfoDto AccountInfo { get; set; }
}
public sealed class AccountInfoDto
{
    public string UserName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
}

internal sealed class LoginCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        AppUser? user=await userManager.Users.FirstOrDefaultAsync(u => u.Email == request.EmailOrUserName || u.UserName == request.EmailOrUserName, cancellationToken);

        if(user is null)
        {
            return Result<LoginCommandResponse>.Failure("Kullanıcı bulunamadı!");
        }

        SignInResult signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (signInResult.IsLockedOut)
        {
            TimeSpan? timeSpan = user.LockoutEnd - DateTime.UtcNow;
            if (timeSpan.HasValue)
            {
                return Result<LoginCommandResponse>.Failure($"Kullanıcı kilitli! {timeSpan.Value.TotalMinutes} dakika sonra tekrar deneyiniz.");
            }
            else
            {
                return Result<LoginCommandResponse>.Failure("Kullanıcı kilitli! Lütfen daha sonra tekrar deneyiniz.");
            }
        }
        
        if(signInResult.IsNotAllowed)
        {
            if (!user.EmailConfirmed)
            {
                return Result<LoginCommandResponse>.Failure("E-posta adresiniz henüz doğrulanmadı. Lütfen e-postanıza gelen bağlantıya tıklayarak hesabınızı doğrulayın.");
            }
            return Result<LoginCommandResponse>.Failure("Kullanıcı giriş yapmaya izin verilmiyor! Lütfen yöneticinizle iletişime geçiniz.");
        }
        if (!signInResult.Succeeded)
        {
            return Result<LoginCommandResponse>.Failure("Kullanıcı adı veya şifre hatalı!");
        }
        var userRoles=await userManager.GetRolesAsync(user);
        var userRole=userRoles.FirstOrDefault();
        var accessToken = await jwtProvider.CreateTokenAsync(user,userRole!);

        var response= new LoginCommandResponse
        {
            AccessToken = accessToken,
            AccountInfo = new()
            {
                UserName=user.UserName!,
                UserRole=userRole!

            }
        };
        return Result<LoginCommandResponse>.Success(response, "Giriş başarılı!");
    }
}

