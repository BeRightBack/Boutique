using System;
using System.Collections.Generic;

namespace Boutique.Models;

public class ProductViewModel
{
    public ProductViewModel()
    {
        ProductImages = new List<string>();
        Categories = new List<CategoryViewModel>();
        Manufacturers = new List<ManufacturerViewModel>();
        Specifications = new List<SpecificationViewModel>();
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public decimal OldPrice { get; set; }

    public string Description { get; set; }

    public string MainImage { get; set; }

    public List<string> ProductImages { get; set; }

    public string SeoUrl { get; set; }

    public string MetaTitle { get; set; }

    public string MetaKeywords { get; set; }

    public string MetaDescription { get; set; }

    public decimal Rating { get; set; }

    public int ReviewCount { get; set; }

    public DateTime DateAdded { get; set; }

    public List<CategoryViewModel> Categories { get; set; }

    public List<ManufacturerViewModel> Manufacturers { get; set; }

    public List<SpecificationViewModel> Specifications { get; set; }

}
