using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Users;
public sealed record GetUsersAllQuery(
    int Page,
    int PageSize,
    string? SearchTerm
    
    ) : IRequest<Result<GetUsersAllQueryResponse>>;


public sealed record GetUsersAllQueryResponse()
{
    public List<UserDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
public sealed class UserDto : EntityDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }

}

internal sealed class GetUsersQueryHandler(
    UserManager<AppUser> userManager
    ) : IRequestHandler<GetUsersAllQuery, Result<GetUsersAllQueryResponse>>
{
    public async Task<Result<GetUsersAllQueryResponse>> Handle(GetUsersAllQuery request, CancellationToken cancellationToken)
    {
        var totalCount = userManager.Users.Count();

        var pagedUsers = await (
            from user in userManager.Users
            join creator in userManager.Users
                on user.CreateUserId equals creator.Id into createdByGroup
            from createdBy in createdByGroup.DefaultIfEmpty()
            where string.IsNullOrEmpty(request.SearchTerm)
                ||  user.Email!.Contains(request.SearchTerm)
                || user.UserName!.Contains(request.SearchTerm)
            select new UserDto
            {
                Id=user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                CreateUserName = createdBy.UserName,
                UpdatedAt = user.UpdatedAt,
                UpdateUserId = user.UpdateUserId,
                DeletedAt = user.DeletedAt,
                DeleteUserId = user.DeleteUserId,
            })
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        GetUsersAllQueryResponse response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Users = pagedUsers,
        };

        return Result<GetUsersAllQueryResponse>.Success(response, "successfully taked users with pagination!");


    }
}
