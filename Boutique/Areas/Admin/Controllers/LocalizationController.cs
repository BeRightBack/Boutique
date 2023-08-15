using Boutique.Areas.Admin.Models;
using Boutique.Data;
using Boutique.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class LocalizationController : Controller
{
    private readonly LocalizationDbContext _context;

    public LocalizationController(LocalizationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 12)
    {
        var stringResourceQuery = from s in _context.StringResources select s;

        // Filter based on search string
        if (!string.IsNullOrEmpty(searchString))
        {
            stringResourceQuery = stringResourceQuery.Where(s => s.Name.Contains(searchString) || s.Value.Contains(searchString));
        }

        // Get total count of filtered results
        var totalCount = await stringResourceQuery.CountAsync();

        // Calculate number of pages
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        // Ensure page number is within valid range
        if (page < 1)
        {
            page = 1;
        }
        else if (page > totalPages)
        {
            page = totalPages;
        }

        // Select only the requested page
        var stringResources = await stringResourceQuery
            .OrderBy(sr => sr.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(sr => new LocalizationViewModel
            {
                Id = sr.Id,
                Name = sr.Name,
                Value = sr.Value,
                LanguageName = sr.Language.Name,
            })
            .ToListAsync();

        // Add paging information to ViewBag for use in view
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = totalPages;
        ViewBag.SearchString = searchString;

        return View(stringResources);
    }


    // GET: Admin/Localization/Create
    public IActionResult Create()
    {
        var viewModel = new StringResourceViewModel();
        return View(viewModel);
    }

    // POST: Admin/Localization/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StringResourceViewModel stringResourceViewModel)
    {
        if (ModelState.IsValid)
        {
            var stringResource = new StringResource
            {
                LanguageId = stringResourceViewModel.LanguageId,
                Name = stringResourceViewModel.Name,
                Value = stringResourceViewModel.Value
            };

            _context.Add(stringResource);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        return View(stringResourceViewModel);
    }

    // GET: Admin/Localization/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var stringResource = await _context.StringResources.FindAsync(id);

        if (stringResource == null)
        {
            return NotFound();
        }

        var viewModel = new StringResourceViewModel
        {
            Id = stringResource.Id,
            LanguageId = (int)stringResource.LanguageId,
            Name = stringResource.Name,
            Value = stringResource.Value
        };

        return View(viewModel);
    }

    // POST: Admin/Localization/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StringResourceViewModel stringResourceViewModel)
    {
        if (ModelState.IsValid)
        {
            var stringResource = new StringResource
            {
                Id = stringResourceViewModel.Id,
                LanguageId = stringResourceViewModel.LanguageId,
                Name = stringResourceViewModel.Name,
                Value = stringResourceViewModel.Value
            };
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
        return View(stringResourceViewModel);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var stringResource = await _context.StringResources
            .FirstOrDefaultAsync(m => m.Id == id);
        if (stringResource == null)
        {
            return NotFound();
        }

        var viewModel = new StringResourceViewModel
        {
            Id = stringResource.Id,
            LanguageId = (int)stringResource.LanguageId,
            Name = stringResource.Name,
            Value = stringResource.Value
        };

        return View(viewModel);
    }

    // GET: Admin/Localization/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var stringResource = await _context.StringResources
            .FirstOrDefaultAsync(m => m.Id == id);
        if (stringResource == null)
        {
            return NotFound();
        }

        var viewModel = new StringResourceViewModel
        {
            Id = stringResource.Id,
            LanguageId = (int)stringResource.LanguageId,
            Name = stringResource.Name,
            Value = stringResource.Value
        };

        return View(viewModel);
    }

    // POST: Admin/Localization/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var stringResourceViewModel = await _context.StringResources
            .Where(sr => sr.Id == id)
            .Select(sr => new StringResourceViewModel
            {
                Id = sr.Id,
                LanguageId = (int)sr.LanguageId,
                Name = sr.Name,
                Value = sr.Value

                // Map other properties as needed
            })
            .FirstOrDefaultAsync();

        if (stringResourceViewModel == null)
        {
            return NotFound();
        }

        _context.StringResources.Remove(stringResourceViewModel.ToEntity());
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool StringResourceExists(int id)
    {
        return _context.StringResources.Any(e => e.Id == id);
    }
}
