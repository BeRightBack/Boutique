
using System.ComponentModel.DataAnnotations;

namespace Boutique.Models.ManageViewModels;
public class DeletePersonalDataViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RequirePassword { get; set; }
}
