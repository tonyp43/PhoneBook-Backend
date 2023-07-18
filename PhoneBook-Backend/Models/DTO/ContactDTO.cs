using System.ComponentModel.DataAnnotations;

namespace PhoneBook_Backend.Models.DTO;

public class ContactDTO
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string SocialNetworkLink { get; set; }
}