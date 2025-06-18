using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public string RoleName { get; set; } = default!;

}

internal sealed class GetUsersQueryHandler(
    UserManager<AppUser> userManager
    ) : IRequestHandler<GetUsersAllQuery, Result<GetUsersAllQueryResponse>>
{
    public async Task<Result<GetUsersAllQueryResponse>> Handle(GetUsersAllQuery request, CancellationToken cancellationToken)
    {

        var query =
            from user in userManager.Users
            join creator in userManager.Users
                on user.CreateUserId equals creator.Id into createdByGroup
            from createdBy in createdByGroup.DefaultIfEmpty()
            where string.IsNullOrEmpty(request.SearchTerm)
                || user.Email!.Contains(request.SearchTerm)
                || user.UserName!.Contains(request.SearchTerm)
            select new
            {
                User = user,
                CreatedBy = createdBy,
            };

        var totalCount =await query.CountAsync(cancellationToken);

        var pagedQuery = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        var pagedUsersData=await pagedQuery.ToListAsync(cancellationToken);
        var userDtos=new List<UserDto>();

        foreach (var item in pagedUsersData)
        {
            var userRoles = await userManager.GetRolesAsync(item.User);

            userDtos.Add(new UserDto
            {
                Id = item.User.Id,
                UserName = item.User.UserName,
                FirstName = item.User.FirstName,
                LastName = item.User.LastName,
                Email = item.User.Email,
                CreatedAt = item.User.CreatedAt,
                CreateUserName = item.CreatedBy.UserName,
                UpdatedAt = item.User.UpdatedAt,
                UpdateUserId = item.User.UpdateUserId,
                DeletedAt = item.User.DeletedAt,
                DeleteUserId = item.User.DeleteUserId,
                RoleName = userRoles.First()
            });
        }
       

        GetUsersAllQueryResponse response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Users = userDtos,
        };

        return Result<GetUsersAllQueryResponse>.Success(response, "successfully taked users with pagination!");


    }
}
