using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Utilities;

namespace PhoneBook_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtService _jwtService;
    private readonly IMailService _mailService;

    public UserController(UserManager<IdentityUser> userManager, JwtService jwtService, IMailService mailService) {
        _userManager = userManager;
        _jwtService = jwtService;
        _mailService = mailService;
    }
    
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userManager.CreateAsync(
            new IdentityUser() { UserName = user.UserName, Email = user.Email },
            user.Password
        );

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        WelcomeEmail(user.Email, user.UserName);

        user.Password = null;
        return CreatedAtAction("GetUser", new { username = user.UserName }, user);
    }
    
    [Authorize]
    [HttpGet("{username}")]
    public async Task<ActionResult<User>> GetUser(string username)
    {
        IdentityUser user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        return new User
        {
            UserName = user.UserName,
            Email = user.Email
        };
    }
    
    [HttpPost("BearerToken")]
    public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Bad credentials");
        }

        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user == null)
        {
            return BadRequest("Bad credentials");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            return BadRequest("Bad credentials");
        }

        var token = _jwtService.CreateToken(user);

        return Ok(token);
    }

    private void WelcomeEmail(string email, string name)
    {
        MailRequest mailRequest = new MailRequest();
        mailRequest.ToEmail = email;
        mailRequest.Subject = "Welcome to Phonebook Application!";
        mailRequest.Body = "Thank you for registering to this Phonebook application, " + name + "!";
        _mailService.SendEmailAsync(mailRequest);
    }
}