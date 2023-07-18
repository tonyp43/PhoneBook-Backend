using Microsoft.AspNetCore.Identity;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Models.DTO;

namespace PhoneBook_Backend.Services.IServices;

public interface IContactService
{
    Task<Contact> CreateContact(ContactDTO contactDto, IdentityUser user);
    Task<bool> DeleteContact(int id, IdentityUser user);
    Task<IEnumerable<Contact>> GetContacts(IdentityUser user);
    Task<Contact> GetContact(int id, IdentityUser user);
    Task<bool> UpdateContact(int id, ContactDTO contactDto, IdentityUser user);
}
