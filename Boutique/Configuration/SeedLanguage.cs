using System.Linq;
using System.Threading.Tasks;
using Boutique.Data;
using Boutique.Entity;

namespace Boutique.Configuration;
public class SeedLanguage
{
    private static LocalizationDbContext _context;

    public SeedLanguage(LocalizationDbContext context)
    {
        _context = context;
    }

    public static Task EnsureSeedLanguageAsync()
    {

        if (!_context.Languages.Any())
        {
            var _language_fr = new Language
            {
                Name = "Français",
                Culture = "fr"
            };
            var _language_en = new Language
            {
                Name = "English",
                Culture = "en"
            };
            var _language_es = new Language
            {
                Name = "Español",
                Culture = "es"
            };
            _context.Languages.Add(_language_fr);
            _context.Languages.Add(_language_en);
            _context.Languages.Add(_language_es);
            _context.SaveChanges();
        }
        return Task.CompletedTask;
    }

}