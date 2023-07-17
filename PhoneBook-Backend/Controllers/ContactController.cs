using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Repository;
using PhoneBook_Backend.Repository.IRepository;

namespace PhoneBook_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;

    public ContactController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    [Authorize]
    [HttpPost("CreateContact")]
    public async Task<IActionResult> CreateContact(ContactDTO contactDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        // Get the currently authenticated user
        ClaimsPrincipal activeUser = HttpContext.User;
        IdentityUser user = await _userManager.FindByNameAsync(activeUser.Identity.Name);
        if (user == null)
        {
            return Unauthorized();
        }
        
        //Create a new contact object from the data received
        Contact contact = new Contact
        {
            FirstName = contactDto.FirstName,
            LastName = contactDto.LastName,
            Email = contactDto.Email,
            PhoneNumber = contactDto.PhoneNumber,
            SocialNetworkLink = contactDto.SocialNetworkLink,
            UserId = user.Id,
            User = user
        };
        
        _unitOfWork.Contact.Add(contact);
        _unitOfWork.Save();
        return Ok(contact);
    }

    [Authorize]
    [HttpDelete("DeleteContact")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        ClaimsPrincipal activeUser = HttpContext.User;
        IdentityUser user = await _userManager.FindByNameAsync(activeUser.Identity.Name);
     
        if (user == null)
        {
            return Unauthorized();
        }
        
        var contactToBeDeleted = _unitOfWork.Contact.Get(u=>u.Id==id);

        if (GetOwnership(user, contactToBeDeleted))
        {
            _unitOfWork.Contact.Delete(contactToBeDeleted);
            _unitOfWork.Save();
            
            return Ok(new { success = true, message = "Delete Successful" });
        }

        return BadRequest();
    }

    [Authorize]
    [HttpGet("GetContacts")]
    public async Task<IActionResult> GetContacts()
    {
        ClaimsPrincipal activeUser = HttpContext.User;
        IdentityUser user = await _userManager.FindByNameAsync(activeUser.Identity.Name);
        
        if (user == null)
        {
            return Unauthorized();
        }

        var contacts = _unitOfWork.Contact.GetAll(includeProperties: "User").Where(c => c.UserId == user.Id);
        
        return Ok(contacts);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetContact(int id)
    {
        ClaimsPrincipal activeUser = HttpContext.User;
        IdentityUser user = await _userManager.FindByNameAsync(activeUser.Identity.Name);
        
        if (user == null)
        {
            return Unauthorized();
        }

        var contact = _unitOfWork.Contact.Get(u=>u.Id==id);

        if (!GetOwnership(user, contact))
        {
            return BadRequest();
        }

        return contact;
    }
    
    [Authorize]
    [HttpPost("Update")]
    public async Task<ActionResult<Contact>> Update(int id, ContactDTO contactDto)
    {
        ClaimsPrincipal activeUser = HttpContext.User;
        IdentityUser user = await _userManager.FindByNameAsync(activeUser.Identity.Name);
        
        if (user == null)
        {
            return Unauthorized();
        }

        var contact = _unitOfWork.Contact.Get(u=>u.Id==id);

        if (!GetOwnership(user, contact))
        {
            return BadRequest();
        }

        contact.FirstName = contactDto.FirstName;
        contact.LastName = contactDto.LastName;
        contact.Email = contactDto.Email;
        contact.PhoneNumber = contactDto.PhoneNumber;
        contact.SocialNetworkLink = contactDto.SocialNetworkLink;

        _unitOfWork.Contact.Update(contact);
        _unitOfWork.Save();

        return Ok();
    }

    //Determine if the user has ownership of the specified target
    private bool GetOwnership(IdentityUser caller, Contact target)
    {
        if (target.UserId == caller.Id)
        {
            return true;
        }

        return false;
    }
}