using System.Collections.Generic;
using Boutique.Entity;

namespace Boutique.Services;

public interface ILanguageService
{
    IEnumerable<Language> GetLanguages();
    Language GetLanguageByCulture(string culture);
}
