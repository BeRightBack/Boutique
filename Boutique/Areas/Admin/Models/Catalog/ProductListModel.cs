namespace Boutique.Areas.Admin.Models;

public class ProductListModel
{
    public Guid Id { get; set; }
    public string MainImage { get; set; }
    public string Name { get; set; }
    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool Published { get; set; }
}
