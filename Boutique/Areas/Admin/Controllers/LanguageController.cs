using Boutique.Data;
using Boutique.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class LanguageController : Controller
{
    private readonly LocalizationDbContext _context;

    public LanguageController(LocalizationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Language
    public async Task<IActionResult> Index()
    {
        return View(await _context.Languages.ToListAsync());
    }

    // GET: Admin/Language/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var language = await _context.Languages
            .FirstOrDefaultAsync(m => m.Id == id);
        if (language == null)
        {
            return NotFound();
        }

        return View(language);
    }

    // GET: Admin/Language/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Language/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Culture")] Language language)
    {
        if (ModelState.IsValid)
        {
            _context.Add(language);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(language);
    }

    // GET: Admin/Language/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var language = await _context.Languages.FindAsync(id);
        if (language == null)
        {
            return NotFound();
        }
        return View(language);
    }

    // POST: Admin/Language/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Culture")] Language language)
    {
        if (id != language.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(language);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(language.Id))
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
        return View(language);
    }

    // GET: Admin/Language/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var language = await _context.Languages
            .FirstOrDefaultAsync(m => m.Id == id);
        if (language == null)
        {
            return NotFound();
        }

        return View(language);
    }

    // POST: Admin/Language/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var language = await _context.Languages.FindAsync(id);
        if (language != null)
        {
            _context.Languages.Remove(language);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LanguageExists(int id)
    {
        return _context.Languages.Any(e => e.Id == id);
    }
}
