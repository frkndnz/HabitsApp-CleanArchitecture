using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Users;
public sealed record GetUserProfileQuery():IRequest<Result<GetUserProfileQueryResponse>>;


public sealed record GetUserProfileQueryResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = default!;
    public string Email {  get; set; } = default!; 
    public string FirstName { get; set; }= default!;
    public string LastName { get; set; }=default!;
    public DateTime CreatedAt { get; set; }
}

internal sealed class GetUserProfileQueryHandler
    (
        UserManager<AppUser> userManager,
        ICurrentUserService currentUserService,
        IMapper mapper
    ) : IRequestHandler<GetUserProfileQuery, Result<GetUserProfileQueryResponse>>
{
    public async Task<Result<GetUserProfileQueryResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        bool isExist=userManager.Users.Any(u=>u.Id==userId);
        if (!isExist)
            return Result<GetUserProfileQueryResponse>.Failure("user not found!");

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        var result=mapper.Map<GetUserProfileQueryResponse>(user);

        return Result<GetUserProfileQueryResponse>.Success(result,"retrieved user info!");
    }
}
