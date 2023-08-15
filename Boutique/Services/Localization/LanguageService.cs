using Boutique.Data;
using Boutique.Entity;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Boutique.Services;
public class LanguageService : ILanguageService
{
    private readonly LocalizationDbContext _context;

    public LanguageService(LocalizationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Language> GetLanguages()
    {
        return _context.Languages.ToList();
    }

    public Language GetLanguageByCulture(string culture)
    {
        return _context.Languages.FirstOrDefault(x =>
            x.Culture.Trim().ToLower() == culture.Trim().ToLower());
    }
}