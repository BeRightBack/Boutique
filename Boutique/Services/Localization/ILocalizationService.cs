using Boutique.Entity;

namespace Boutique.Services;

public interface ILocalizationService
{
    StringResource GetStringResource(string resourceKey, int languageId);
    void AddOrUpdateStringResource(StringResource resource);
}
