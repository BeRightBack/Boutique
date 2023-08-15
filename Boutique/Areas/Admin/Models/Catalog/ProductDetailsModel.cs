using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Boutique.Areas.Admin.Models;

public class ProductDetailsModel
{
    public Guid Id { get; set; }
    public string MainImage { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string SKU { get; set; }
    public string ShortDescription { get; set; }
    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal? SpecialPrice { get; set; }    
    public DateTime? SpecialPriceStartDate { get; set; }
    public DateTime? SpecialPriceEndDate { get; set; }
    public int StockQuantity { get; set; }
    public int? MinimumStockQuantity { get; set; }
    public int? NotifyForQuantityBelow { get; set; }
    public bool DisplayAvailability { get; set; }
    public int? MinimumCartQuantity { get; set; }
    public int? MaximumCartQuantity { get; set; }

    [Display(Name = "SEO Url")]
    [RegularExpression(@"^[a-zA-Z0-9]+(-[a-zA-Z0-9]+)*$", ErrorMessage = "URL must only contains alphanumeric [a-z A-Z 0-9] and dash [-] e.g. abc-123-D45")]
    public string SeoUrl { get; set; }

    [Display(Name = "Meta Tag Title")]
    public string MetaTitle { get; set; }

    [Display(Name = "Meta Tag Keywords")]
    public string MetaKeywords { get; set; }

    [Display(Name = "Meta Tag Description")]
    public string MetaDescription { get; set; }

    [Display(Name = "Category")]
    public List<string> CategoryIds { get; set; }
    public SelectList CategorySelectList { get; set; }

    [Display(Name = "Images")]
    public List<ImageModel> Images { get; set; }

    [Display(Name = "Images Ids")]
    public List<string> ImageIds { get; set; }
    public List<string> ImageSortOrder { get; set; }
    
    [Display(Name = "Manufacturer")]
    public List<string> ManufacturerIds { get; set; }
    public SelectList ManufacturerSelectList { get; set; }
    public List<ProductSpecificationModel> Specifications { get; set; }
    public SelectList SpecificationKeySelectList { get; set; }
    public bool Published { get; set; }
    public DateTime DateAdded { get; set; }
}
