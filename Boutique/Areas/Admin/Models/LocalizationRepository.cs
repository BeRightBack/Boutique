using Boutique.Data;
using Boutique.Entity;
using Boutique.Models;
using Boutique.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Areas.Admin.Models;

public class LocalizationRepository : ILocalizationRepository
{
    private readonly LocalizationDbContext context;
    public LocalizationRepository(LocalizationDbContext ctx) => context = ctx;

    public async Task<StringResource> GetLocalizationByIdAsync(int id)
    {
        return await context.StringResources.FindAsync(id);
    }

    public async Task<IEnumerable<StringResource>> GetAllLocalizationsAsync()
    {
        return await context.StringResources.ToListAsync();
    }

    public PagedList<StringResource> GetLocalizations(QueryOptions options)
    {
        return new PagedList<StringResource>(context.StringResources, options);
    }

    public async Task<StringResource> CreateLocalizationAsync(StringResource localization)
    {
        context.Add(localization);
        await context.SaveChangesAsync();
        return localization;
    }

    public async Task UpdateLocalizationAsync(StringResource localization)
    {
        context.Update(localization);
        await context.SaveChangesAsync();
    }

    public async Task DeleteLocalizationAsync(StringResource localization)
    {
        context.Remove(localization);
        await context.SaveChangesAsync();
    }
}