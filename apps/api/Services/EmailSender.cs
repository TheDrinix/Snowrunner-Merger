using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SnowrunnerMerger.Api.Models.Auth;

namespace SnowrunnerMerger.Api.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly IConfiguration _config;
    public EmailSender(ILogger<EmailSender> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }
    
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mailConfigSlice = _config.GetSection("SMTP");
        if (mailConfigSlice is null)
        {
            throw new ArgumentNullException(nameof(mailConfigSlice));
        }
        
        var mailConfig = mailConfigSlice.Get<MailConfig>();
        if (mailConfig is null)
        {
            throw new ArgumentNullException(nameof(mailConfig));
        }

        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress("Snowrunner Merger", mailConfig.Address));
        msg.To.Add(new MailboxAddress("", email));
        msg.Subject = subject;
        
        msg.Body = new TextPart("html")
        {
            Text = message
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(mailConfig.Host, mailConfig.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(mailConfig.Username, mailConfig.Password);
        
        await client.SendAsync(msg);
        await client.DisconnectAsync(true);
    }
}