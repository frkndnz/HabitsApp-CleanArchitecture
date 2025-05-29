
using HabitsApp.Application.Services;
using MimeKit;
using MailKit.Net.Smtp;
namespace HabitsApp.Infrastructure.Services;
internal class EmailService : IEmailService
{
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
        await smtp.AuthenticateAsync("sahagurusu@gmail.com", "mypedniettketsdh");
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
