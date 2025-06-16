using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using HabitsApp.Application.Services;
using Microsoft.Extensions.Configuration;

namespace HabitsApp.Infrastructure.Services;
internal class GoogleAuthValidator : IGoogleAuthValidator
{
    private readonly string _googleClientId;

    public GoogleAuthValidator(IConfiguration configuration)
    {
        _googleClientId = configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("Google Client ID not configured.");
    }
    public async Task<GoogleUserPayload?> ValidateGoogleIdTokenAsync(string googleIdToken)
    {
        try
        {
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new string[] { _googleClientId }
            };

            var payload=await GoogleJsonWebSignature.ValidateAsync(googleIdToken, validationSettings);

            return new GoogleUserPayload
            {
                Subject = payload.Subject,
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                Name = payload.Name,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName,
                Picture = payload.Picture,
                Locale = payload.Locale,
               
                
            };
        }
        catch (InvalidJwtException ex)
        {

            Console.WriteLine($"Google ID Token doğrulaması başarısız: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Google ID Token doğrulanırken hata oluştu: {ex.Message}");
            return null;
        }
    }
}
