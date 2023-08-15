namespace Boutique.Areas.Admin.Models;

public class CategoryListModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameWithParent { get; set; }
    public bool Published { get; set; }
}
