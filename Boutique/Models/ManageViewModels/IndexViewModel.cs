using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Boutique.Models.ManageViewModels;
public class IndexViewModel
{
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Profile Picture")]
    public byte[] ProfilePicture { get; set; }

    [Display(Name = "Profile Picture Path")]
    public string ProfilePicturePath { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile ProfilePictureFile { get; set; }

    public string StatusMessage { get; set; }
    public string UserNameChangeLimitMessage { get; set; }
}
