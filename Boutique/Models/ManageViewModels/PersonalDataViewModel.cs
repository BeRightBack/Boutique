using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace Boutique.Models.ManageViewModels;
public class PersonalDataViewModel
{
    public string UserId { get; set; }
    public string Username { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }

    public bool IsPhoneNumberConfirmed { get; set; }

    [Display(Name = "Profile Picture")]
    public byte[] ProfilePicture { get; set; }

    public bool TwoFactor { get; set; }
    public IList<UserLoginInfo> Logins { get; set; }

    public IList<UserLoginInfo> ExternalLogins { get; set; }

    [Display(Name = "Profile Picture Path")]
    public string ProfilePicturePath { get; set; }

    public string StatusMessage { get; set; }
}
