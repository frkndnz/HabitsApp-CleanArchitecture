
using HabitsApp.Application.Services;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
namespace HabitsApp.Infrastructure.Services;
internal class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("sahagurusu@gmail.com"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = body
        };  


        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("sahagurusu@gmail.com", _configuration["Gmail:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
