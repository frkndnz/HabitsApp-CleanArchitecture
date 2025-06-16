using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitsApp.Application.Services;
public interface IGoogleAuthValidator
{
    Task<GoogleUserPayload?> ValidateGoogleIdTokenAsync(string googleIdToken);
}

public class GoogleUserPayload
{
    public string Subject { get; set; } = string.Empty; // Google'ın kullanıcı ID'si
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty; // Profil resmi URL'si
    public string Locale { get; set; } = string.Empty;
    
}