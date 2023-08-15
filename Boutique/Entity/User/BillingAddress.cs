using System;

namespace Boutique.Entity;

public class BillingAddress
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string StateProvince { get; set; }
    public string ZipPostalCode { get; set; }
    public string Country { get; set; }
    public string Telephone { get; set; }
}
