using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneBook_Backend.Models;
using PhoneBook_Backend.Services;
using PhoneBook_Backend.Services.IServices;

namespace PhoneBook_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var createdUser = await _userService.CreateUser(user);

            if (createdUser == null)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<User>> GetUser(string username)
        {
            var user = await _userService.GetUser(username);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("BearerToken")]
        public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
        {
            var token = await _userService.CreateBearerToken(request);

            if (token == null)
            {
                return BadRequest("Bad credentials");
            }

            return Ok(token);
        }
    }
}