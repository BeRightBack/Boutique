using System;
using Boutique.Models;
using Microsoft.AspNetCore.Identity;

namespace Boutique.Entity;

public class ApplicationUser : IdentityUser
{
    [LocalizedDisplayName("FirstName")]
    public string FirstName { get; set; }
    [LocalizedDisplayName("LastName")]
    public string LastName { get; set; }
    public int UsernameChangeLimit { get; set; } = 10;
    public byte[] ProfilePicture { get; set; }
    public string ProfilePicturePath { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public Guid BillingAddressId { get; set; }
}

