using Boutique.Data;
using Boutique.Entity;
using System.Linq;

namespace Boutique.Services;
public class LocalizationService : ILocalizationService
{
    private readonly LocalizationDbContext _context;

    public LocalizationService(LocalizationDbContext context)
    {
        _context = context;
    }

    public StringResource GetStringResource(string resourceKey, int languageId)
    {
        return _context.StringResources.FirstOrDefault(x =>
                x.Name.Trim().ToLower() == resourceKey.Trim().ToLower()
                && x.LanguageId == languageId);
    }

    public void AddOrUpdateStringResource(StringResource resource)
    {
        lock (_context)
        {
            _context.Add(resource);
            _context.SaveChanges();
        }
    }
}
