using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PhoneBook_Backend.Configuration;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Services.IServices;

namespace PhoneBook_Backend.Services;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        
        email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
        email.Subject = mailRequest.Subject;
        
        var builder = new BodyBuilder();
        
        if (mailRequest.Attachments != null)
        {
            foreach (var file in mailRequest.Attachments.Where(file => file.Length > 0))
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                builder.Attachments.Add(file.FileName, ms.ToArray(), ContentType.Parse(file.ContentType));
            }
        }
        builder.HtmlBody = mailRequest.Body;
        email.Body = builder.ToMessageBody();
        
        using var smtp = new SmtpClient();
        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}