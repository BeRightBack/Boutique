using Boutique.Data;
using Boutique.Entity;
using Boutique.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class LocationController : Controller
{
    private readonly LocalizationDbContext _context;

    public LocationController(LocalizationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Location
    public async Task<IActionResult> Index(string searchString, string currentFilter, string sortOrder, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["LangSortParm"] = sortOrder == "Language" ? "lang_desc" : "Language";
        ViewData["CurrentFilter"] = searchString;

        var stringResources = from s in _context.StringResources select s;

        if (!String.IsNullOrEmpty(searchString))
        {
            stringResources = stringResources.Where(s => s.Name.Contains(searchString)
                                       || s.Value.Contains(searchString));
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        stringResources = sortOrder switch
        {
            "name_desc" => stringResources.OrderByDescending(s => s.Name),
            "Language" => stringResources.OrderBy(s => s.Language.Name),
            "lang_desc" => stringResources.OrderByDescending(s => s.Language.Name),
            _ => stringResources.OrderBy(s => s.Name),
        };
        int pageSize = 12;
        var paginatedStringResources = await PaginatedList<StringResource>.CreateAsync(stringResources.AsNoTracking().Include(s => s.Language), pageNumber ?? 1, pageSize);
        return View(paginatedStringResources);
    }


    // GET: Admin/Location/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.StringResources == null)
        {
            return NotFound();
        }

        var stringResource = await _context.StringResources
            .Include(s => s.Language)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (stringResource == null)
        {
            return NotFound();
        }

        return View(stringResource);
    }

    // GET: Admin/Location/Create
    public IActionResult Create()
    {
        ViewData["LanguageId"] = new SelectList(_context.Set<Language>(), "Id", "Id");
        return View();
    }

    // POST: Admin/Location/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,LanguageId,Name,Value")] StringResource stringResource)
    {
        if (ModelState.IsValid)
        {
            _context.Add(stringResource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["LanguageId"] = new SelectList(_context.Set<Language>(), "Id", "Id", stringResource.LanguageId);
        return View(stringResource);
    }

    // GET: Admin/Location/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.StringResources == null)
        {
            return NotFound();
        }

        var stringResource = await _context.StringResources.FindAsync(id);
        if (stringResource == null)
        {
            return NotFound();
        }
        ViewData["LanguageId"] = new SelectList(_context.Set<Language>(), "Id", "Id", stringResource.LanguageId);
        return View(stringResource);
    }

    // POST: Admin/Location/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,LanguageId,Name,Value")] StringResource stringResource)
    {
        if (id != stringResource.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(stringResource);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StringResourceExists(stringResource.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["LanguageId"] = new SelectList(_context.Set<Language>(), "Id", "Id", stringResource.LanguageId);
        return View(stringResource);
    }

    // GET: Admin/Location/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.StringResources == null)
        {
            return NotFound();
        }

        var stringResource = await _context.StringResources
            .Include(s => s.Language)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (stringResource == null)
        {
            return NotFound();
        }

        return View(stringResource);
    }

    // POST: Admin/Location/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.StringResources == null)
        {
            return Problem("Entity set 'ApplicationDbContext.StringResource'  is null.");
        }
        var stringResource = await _context.StringResources.FindAsync(id);
        if (stringResource != null)
        {
            _context.StringResources.Remove(stringResource);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StringResourceExists(int id)
    {
      return _context.StringResources.Any(e => e.Id == id);
    }
}
