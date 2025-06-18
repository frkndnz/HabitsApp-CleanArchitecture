using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FluentValidation;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Auth;
public sealed record RegisterCommand(string UserName,string FirstName,string LastName,string Email,string Password):IRequest<Result<string>>;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(x=>x.UserName).MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır!");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş olamaz!");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş olamaz!");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Eposta boş olamaz!").EmailAddress().WithMessage("Geçerli bir eposta adresi giriniz!");
        RuleFor(x => x.Email).MinimumLength(5).WithMessage("Eposta en az 5 karakter olmalıdır!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre boş olamaz!");
        RuleFor(x => x.Password).MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır!"); // Şifre uzunluğu için örnek
    }
}
    internal sealed class RegisterCommandHandler(UserManager<AppUser> userManager,IEmailService emailService) : IRequestHandler<RegisterCommand, Result<string>>
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
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,

        };

        var result= await userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Result<string>.Failure(errors);
        }
        await userManager.AddToRoleAsync(newUser, "User");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(newUser); // confirm token
        var encodedToken=HttpUtility.UrlEncode(token); // token url encode

        await emailService.SendEmailAsync(newUser.Email, "HabitsApp Hesap Onayı",
            $"Hesabınızı onaylamak için lütfen <a href='http://localhost:5173/auth/confirm-email?userId={newUser.Id}&token={encodedToken}'>buraya</a> tıklayın.");

        return Result<string>.Success(string.Empty,"Kullanıcı başarıyla oluşturuldu!");  
    }
}

