using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PhoneBook_Backend.Models;

public class Contact
{
    //first name, last name, phone number, email, and social network links.
    [Key]
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string SocialNetworkLink { get; set; }

    public string UserId { get; set; }
    public IdentityUser User { get; set; }

}