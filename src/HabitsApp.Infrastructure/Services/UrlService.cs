using System;
using System.Collections.Generic;
using System.IO;
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

        if (string.IsNullOrWhiteSpace(relativePath))
            return relativePath;

        // Eğer zaten tam URL ise (örn. blob URL), direkt döndür
        if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute))
            return relativePath;

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            return relativePath;

        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}{relativePath}";
    }
}
