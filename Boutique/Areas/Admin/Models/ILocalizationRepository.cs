using Boutique.Entity;
using Boutique.Models;

namespace Boutique.Areas.Admin.Models;

public interface ILocalizationRepository
{
    Task<StringResource> GetLocalizationByIdAsync(int id);
    Task<IEnumerable<StringResource>> GetAllLocalizationsAsync();
    PagedList<StringResource> GetLocalizations(QueryOptions options);
    Task<StringResource> CreateLocalizationAsync(StringResource localization);
    Task UpdateLocalizationAsync(StringResource localization);
    Task DeleteLocalizationAsync(StringResource localization);
}