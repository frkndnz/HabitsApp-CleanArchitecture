using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.Auth;
public sealed class AccountMeQuery:IRequest<Result<AccountMeQueryResponse>>
{
}

public sealed class AccountMeQueryResponse
{
    public string UserName { get; set; } = string.Empty;
    public string UserRole {  get; set; } = string.Empty;
}

internal sealed class AccountMeQueryHandler : IRequestHandler<AccountMeQuery, Result<AccountMeQueryResponse>>
{
    private readonly ICurrentUserService _currentUserService;

    public AccountMeQueryHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public Task<Result<AccountMeQueryResponse>> Handle(AccountMeQuery request, CancellationToken cancellationToken)
    {
        var response = new AccountMeQueryResponse
        {
            UserRole = _currentUserService.UserRole,
            UserName = _currentUserService.UserName,
        };

        
        return Task.FromResult(Result<AccountMeQueryResponse>.Success(response, "success"));
    }
}
