using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Boutique.Areas.Editor.Models;

public class ContentViewModel
{    
    public int Id { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; }

    public string HtmlContent { get; set; }
}
