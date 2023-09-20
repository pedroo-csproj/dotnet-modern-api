using DotNETModernAPI.Domain.Models;
using DotNETModernAPI.Domain.Providers;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DotNETModernAPI.Infrastructure.Providers;

public class EmailProvider : IEmailProvider
{
    public EmailProvider(IOptions<EmailSettingsModel> emailSettings) =>
        _emailSettings = emailSettings.Value;

    private readonly EmailSettingsModel _emailSettings;

    public async Task SendAsync(EmailRequestModel emailRequest)
    {
        var client = BuildSmtpClient(_emailSettings.Host, _emailSettings.Port, _emailSettings.UserName, _emailSettings.Password);
        var mailMessage = BuildMailMessage(_emailSettings.From, emailRequest.To, emailRequest.Subject, emailRequest.Body);

        await client.SendMailAsync(mailMessage);
    }

    // TODO: Inject SmtpClient
    private static SmtpClient BuildSmtpClient(string host, int port, string userName, string password) =>
        new(host, port)
        {
            Credentials = new NetworkCredential(userName, password),
            EnableSsl = true
        };

    // TODO: Inject MailMessage
    private static MailMessage BuildMailMessage(string from, string to, string subject, string body) =>
        new(from, to)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
}
