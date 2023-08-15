using System;

namespace Boutique.Models;

public class CartItemViewModel
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public decimal Price { get; set; }

    public decimal OldPrice { get; set; }

    public int Quantity { get; set; }

    public int MaxCartQuantity { get; set; }

    public string MainImage { get; set; }

    public string SeoUrl { get; set; }
}
