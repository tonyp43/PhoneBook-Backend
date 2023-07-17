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
    
    [HttpGet("TestGet")]
    public IActionResult TestGet()
    {
        return Ok();
    }
    
    [Authorize]
    [HttpGet("TestGetAuthorization")]
    public IActionResult TestGetAuthorization()
    {
        return Ok();
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
    [HttpPost("AuthTest")]
    public async Task<IActionResult> AuthTest()
    {

        
        ClaimsPrincipal activeUser = HttpContext.User;
        // Get the currently authenticated user
        if (activeUser.Identity.IsAuthenticated)
        {
            Console.WriteLine("Current user is " + activeUser.Identity.Name);
            return Ok();
            
        }
        return Unauthorized();
    }
}