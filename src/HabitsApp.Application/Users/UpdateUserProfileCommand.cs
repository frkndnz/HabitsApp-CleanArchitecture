using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Users;
public sealed record UpdateUserProfileCommand(
    Guid Id,
    string? UserName,
    string FirstName,
    string LastName,
    string? Email) :IRequest<Result<string>>;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(x => x.UserName).MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır!");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş olamaz!");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş olamaz!");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Eposta boş olamaz!").EmailAddress().WithMessage("Geçerli bir eposta adresi giriniz!");
        RuleFor(x => x.Email).MinimumLength(5).WithMessage("Eposta en az 5 karakter olmalıdır!");
    }
}

internal sealed class UpdateUserProfileCommandHandler
    (
    UserManager<AppUser> userManager,
    IMapper mapper
    ) : IRequestHandler<UpdateUserProfileCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var exist = await userManager.Users.AnyAsync(u => u.Id == request.Id);
        if (!exist)
            return Result<string>.Failure("user not found!");

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

        mapper.Map(request, user);

        var result = await userManager.UpdateAsync(user!);

        if (!result.Succeeded)
            return Result<string>.Failure("update failed!");

        

        return Result<string>.Success(null, "updated successfully!");
    }
}

