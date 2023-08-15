using Boutique.Entity;

namespace Boutique.Areas.Admin.Models;

public class StringResourceViewModel
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }

    public StringResource ToEntity()
    {
        return new StringResource
        {
            Id = this.Id,
            LanguageId = this.LanguageId,
            Name = this.Name,
            Value = this.Value
        };
    }
}