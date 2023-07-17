using PhoneBook_Backend.Models;

namespace PhoneBook_Backend.Utilities;

public interface IMailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}