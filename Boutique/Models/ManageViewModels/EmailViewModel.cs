using System.ComponentModel.DataAnnotations;

namespace Boutique.Models.ManageViewModels;
public class EmailViewModel
{
    public string Email { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "New email")]
    public string NewEmail { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public string StatusMessage { get; set; }
}