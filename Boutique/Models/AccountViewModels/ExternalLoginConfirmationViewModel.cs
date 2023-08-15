using System.ComponentModel.DataAnnotations;

namespace Boutique.Models.AccountViewModels;
public class ExternalLoginConfirmationViewModel
{
    [Required]
    [Display(Name = "UserName")]
    public string UserName { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

}