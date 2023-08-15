using System;
using System.ComponentModel.DataAnnotations;

namespace Boutique.Models;

public class ReviewViewModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Message { get; set; }
    public int Rating { get; set; }
    public string CreatedOn { get; set; }
    public string DateModified { get; set; }
    public bool IsVerifiedOwner { get; set; }
}
