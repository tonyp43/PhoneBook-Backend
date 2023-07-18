using Microsoft.AspNetCore.Identity;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Models.DTO;
using PhoneBook_Backend.Repository.IRepository;
using PhoneBook_Backend.Services.IServices;

namespace PhoneBook_Backend.Services;

public class ContactService : IContactService
{
    private readonly IUnitOfWork _unitOfWork;

    public ContactService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Contact> CreateContact(ContactDTO contactDto, IdentityUser user)
    {
        var contact = new Contact
        {
            FirstName = contactDto.FirstName,
            LastName = contactDto.LastName,
            Email = contactDto.Email,
            PhoneNumber = contactDto.PhoneNumber,
            SocialNetworkLink = contactDto.SocialNetworkLink,
            Deleted = false,
            UserId = user.Id,
            User = user
        };

        _unitOfWork.Contact.Add(contact);
        _unitOfWork.Save();

        return contact;
    }

    public async Task<bool> DeleteContact(int id, IdentityUser user)
    {
        var contactToBeDeleted = _unitOfWork.Contact.Get(u => (u.Id == id) && (u.Deleted == false));

        if (GetOwnership(user, contactToBeDeleted))
        {
            //_unitOfWork.Contact.Delete(contactToBeDeleted);
            contactToBeDeleted.Deleted = true;
            _unitOfWork.Contact.Update(contactToBeDeleted);
            _unitOfWork.Save();

            return true;
        }

        return false;
    }

    public async Task<IEnumerable<Contact>> GetContacts(IdentityUser user)
    {
        var contacts = _unitOfWork.Contact.GetAll(includeProperties: "User")
            .Where(c => (c.UserId == user.Id) && (c.Deleted == false));

        return contacts;
    }

    public async Task<Contact> GetContact(int id, IdentityUser user)
    {
        var contact = _unitOfWork.Contact.Get(u => (u.Id == id) && (u.Deleted == false));

        if (contact == null)
        {
            return null;
        }
        
        if (!GetOwnership(user, contact))
        {
            return null;
        }

        return contact;
    }

    public async Task<bool> UpdateContact(int id, ContactDTO contactDto, IdentityUser user)
    {
        var contact = _unitOfWork.Contact.Get(u => (u.Id == id) && (u.Deleted == false));

        if (contact == null)
        {
            return false;
        }

        if (!GetOwnership(user, contact))
        {
            return false;
        }

        contact.FirstName = contactDto.FirstName;
        contact.LastName = contactDto.LastName;
        contact.Email = contactDto.Email;
        contact.PhoneNumber = contactDto.PhoneNumber;
        contact.SocialNetworkLink = contactDto.SocialNetworkLink;

        _unitOfWork.Contact.Update(contact);
        _unitOfWork.Save();

        return true;
    }

    private bool GetOwnership(IdentityUser caller, Contact target)
    {
        return target.UserId == caller.Id;
    }
}