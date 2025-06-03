using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using Microsoft.AspNetCore.Http;

namespace HabitsApp.Infrastructure.Services;
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => Guid.TryParse(
        _httpContextAccessor.HttpContext?.User.FindFirst("user_id")!.Value,
        out var userId) ? userId : Guid.Empty;
}
