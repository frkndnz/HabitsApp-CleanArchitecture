using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using HabitsApp.Application.Auth;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Users;
public sealed record UpdateUserCommand(
    Guid Id,
    string? UserName,
    string FirstName,
    string LastName,
    string? Email
    ) : IRequest<Result<UpdateUserCommandResponse>>;


public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(x => x.UserName).MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır!");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş olamaz!");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş olamaz!");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Eposta boş olamaz!").EmailAddress().WithMessage("Geçerli bir eposta adresi giriniz!");
        RuleFor(x => x.Email).MinimumLength(5).WithMessage("Eposta en az 5 karakter olmalıdır!");
    }
}
public sealed record UpdateUserCommandResponse
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }
}


internal sealed class UpdateUserCommandHandler(
    UserManager<AppUser> userManager,
    IMapper mapper

    ) : IRequestHandler<UpdateUserCommand, Result<UpdateUserCommandResponse>>
{
    public async Task<Result<UpdateUserCommandResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var exist = await userManager.Users.AnyAsync(u => u.Id == request.Id);
        if (!exist)
            return Result<UpdateUserCommandResponse>.Failure("user not found!");

        var user= await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

        mapper.Map(request, user);

        var result= await userManager.UpdateAsync(user!);

        if (!result.Succeeded)
            return Result<UpdateUserCommandResponse>.Failure("update failed!");

        var response=mapper.Map<UpdateUserCommandResponse>(user);

        return Result<UpdateUserCommandResponse>.Success(response,"updated successfully!");

        
        
    }
}


