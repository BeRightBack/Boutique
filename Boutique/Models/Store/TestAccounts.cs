namespace Boutique.Models;

public class AdminAccount
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class UserAccount
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
