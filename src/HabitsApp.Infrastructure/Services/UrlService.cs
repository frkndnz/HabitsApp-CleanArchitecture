using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using Microsoft.AspNetCore.Http;

namespace HabitsApp.Infrastructure.Services;
public class UrlService : IUrlService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    
    public string GetAbsoluteUrl(string relativePath)
    {

        var request = _httpContextAccessor.HttpContext?.Request;
        if(request==null)
            return relativePath;

        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}{relativePath}";
    }
}
