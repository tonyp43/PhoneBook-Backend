using PhoneBook_Backend.Models;

namespace PhoneBook_Backend.Services.IServices;

public interface IUserService
{
    Task<User> CreateUser(User user);
    Task<User> GetUser(string username);
    Task<AuthenticationResponse> CreateBearerToken(AuthenticationRequest request);
}