
using System.ComponentModel.DataAnnotations;

namespace Boutique.Models.AccountViewModels;
public class LoginViewModel
{
    [Required]
    [Display(Name = "Email / Username")]
    public string UserName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

}
