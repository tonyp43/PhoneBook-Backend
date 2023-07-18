using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Services;
using PhoneBook_Backend.Services.IServices;

namespace PhoneBook_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly UserManager<IdentityUser> _userManager;

        public ContactController(UserManager<IdentityUser> userManager, IContactService contactService)
        {
            _userManager = userManager;
            _contactService = contactService;
        }

        [Authorize]
        [HttpPost("CreateContact")]
        public async Task<IActionResult> CreateContact(ContactDTO contactDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ClaimsPrincipal activeUser = HttpContext.User;
            IdentityUser user = await _userManager.FindByNameAsync(activeUser.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            var contact = await _contactService.CreateContact(contactDto, user);

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

            var deleted = await _contactService.DeleteContact(id, user);

            if (deleted)
            {
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

            var contacts = await _contactService.GetContacts(user);

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

            var contact = await _contactService.GetContact(id, user);

            if (contact == null)
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

            var updated = await _contactService.UpdateContact(id, contactDto, user);

            if (!updated)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}