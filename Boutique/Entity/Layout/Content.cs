using System.ComponentModel.DataAnnotations;

namespace Boutique.Entity;

public class Content
{
    public int Id { get; set; }
    public string Name { get; set; }
    [DisplayFormat(HtmlEncode = true)] 
    public string HtmlContent { get; set; }
}
