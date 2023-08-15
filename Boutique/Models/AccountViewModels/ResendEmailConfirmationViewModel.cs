using System.ComponentModel.DataAnnotations;

namespace Boutique.Models.AccountViewModels;
public class ResendEmailConfirmationViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}