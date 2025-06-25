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
using Microsoft.Extensions.Configuration;

namespace HabitsApp.Application.Auth;
public sealed record RegisterCommand(string UserName,string FirstName,string LastName,string Email,string Password):IRequest<Result<string>>;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username cannot be empty!");
        RuleFor(x=>x.UserName).MinimumLength(3).WithMessage("Username must be at least 3 characters!");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName cannot be empty!");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName cannot be empty!");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty!").EmailAddress().WithMessage("Please enter a valid email address!");
        RuleFor(x => x.Email).MinimumLength(5).WithMessage("Email must be at least 5 characters!!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be blank!");
        RuleFor(x => x.Password).MinimumLength(8).WithMessage("Password must be at least 8 characters!"); // Şifre uzunluğu için örnek
    }
}
    internal sealed class RegisterCommandHandler(UserManager<AppUser> userManager,IEmailService emailService,IConfiguration configuration) : IRequestHandler<RegisterCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        AppUser? existingUser=await userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName==request.UserName );

        if (existingUser is not null)
        {
            return Result<string>.Failure("An account has already been created with this email or username!");
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

        var url = configuration["Cors:Origin"];
        await emailService.SendEmailAsync(newUser.Email, "HabitsApp Hesap Onayı",
            $"Hesabınızı onaylamak için lütfen <a href='{url}/auth/confirm-email?userId={newUser.Id}&token={encodedToken}'>buraya</a> tıklayın.");

        return Result<string>.Success(string.Empty, "User created successfully!");  
    }
}

