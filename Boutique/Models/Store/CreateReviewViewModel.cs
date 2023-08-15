using System;
using System.ComponentModel.DataAnnotations;
using Boutique.Entity;

namespace Boutique.Models;
#pragma warning disable 8618
public class CreateReviewViewModel
{   
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductSeo { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }

    [Required]
    public int Rating { get; set; }
    public Guid Id { get; }
    public string Name { get; }
    public string SeoUrl { get; }
}
