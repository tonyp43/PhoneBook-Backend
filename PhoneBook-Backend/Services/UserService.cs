using Microsoft.AspNetCore.Identity;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Services.IServices;

namespace PhoneBook_Backend.Services;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtService _jwtService;
    private readonly IMailService _mailService;

    public UserService(UserManager<IdentityUser> userManager, JwtService jwtService, IMailService mailService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _mailService = mailService;
    }

    public async Task<User> CreateUser(User user)
    {

        var result = await _userManager.CreateAsync(
            new IdentityUser() { UserName = user.UserName, Email = user.Email },
            user.Password
        );

        if (!result.Succeeded)
        {
            return null;
        }

        WelcomeEmail(user.Email, user.UserName);

        user.Password = null;
        return user;
    }

    public async Task<User> GetUser(string username)
    {
        IdentityUser user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return null;
        }

        return new User
        {
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<AuthenticationResponse> CreateBearerToken(AuthenticationRequest request)
    {

        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user == null)
        {
            return null;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            return null;
        }

        var token = _jwtService.CreateToken(user);

        return token;
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