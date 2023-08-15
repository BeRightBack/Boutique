using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Boutique.Models;
#pragma warning disable 8618
public class CheckoutViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    [Display(Name = "State/Province")]
    public string StateProvince { get; set; }

    [Required]
    [Display(Name = "Zip/Postal Code")]
    public string ZipPostalCode { get; set; }

    [Required]
    public string Country { get; set; }

    [Required]
    public string Telephone { get; set; }

    public List<CartItemViewModel> CartItemViewModel { get; set; }
}
