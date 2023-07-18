using PhoneBook_Backend.Models;

namespace PhoneBook_Backend.Services.IServices;

public interface IMailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}