using System.Security.Claims;
using Boutique.Areas.Admin.Models.ToDo;
using Boutique.Controllers;
using Boutique.Data;
using Boutique.Entity;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize]
public class ToDoController : BaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public ToDoController(
        ILanguageService languageService,
        ILocalizationService localizationService,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext) : base(languageService, localizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = dbContext;
    }

    public async Task<ActionResult> Index(string id)
    {
        var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        var filters = new Filters(id);
        ViewBag.Filters = filters;
        ViewBag.Categories = await _context.ToDoCategories.ToListAsync();
        ViewBag.Statuses = await _context.ToDoStatuses.ToListAsync();
        ViewBag.DueFilters = Filters.DueFilterValues;
        IQueryable<Todo> query = _context.ToDos
            .Where(todo => todo.User.Id == currentUser.Id)
            .Include(t => t.Category)
            .Include(t => t.Status);
            //.Where(todo => todo.User.Id == currentUser.Id);

        if (filters.HasCategory)
        {
            query = query.Where(todo => todo.CategoryId == filters.CategoryId && todo.User.Id == currentUser.Id);
        }
            

        if (filters.HasDue)
        {
            var today = DateTime.Today;
            if (filters.IsPast)
            {
                query = query.Where(todo => todo.DueDate < today && todo.User.Id == currentUser.Id);
            }
            else if (filters.IsFuture)
            {
                query = query.Where(todo => todo.DueDate > today && todo.User.Id == currentUser.Id);
            }
            else if (filters.IsToday)
            {
                query = query.Where(todo => todo.DueDate == today && todo.User.Id == currentUser.Id);
            }

        }

        if (filters.HasStatus)
        {
            query = query.Where(todo => todo.StatusId == filters.StatusId && todo.User.Id == currentUser.Id);
        }
        var task = await query.OrderBy(t => t.DueDate).ToListAsync();

        //var todos = _context.ToDos.ToList().Where(todo => todo.User.Id == currentUser.Id);

        return View(task);
    }

    [HttpGet]
    public async Task<ActionResult> Create()
    {
        var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        ViewBag.Categories = _context.ToDoCategories.ToList();
        ViewBag.Statuses = _context.ToDoStatuses.ToList();
        var task = new Todo
        {
            User = currentUser,
            StatusId = "open"
        };
        return View(task);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Todo task)
    {
        if (ModelState.IsValid)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            task.User = currentUser;
            _context.ToDos.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.Categories = _context.ToDoCategories.ToList();
            ViewBag.Statuses = _context.ToDoStatuses.ToList();
            return View(task);
        }
    }

    [HttpPost]
    public IActionResult Filter(string[] filter)
    {
        string id = string.Join("-", filter);
        return RedirectToAction("Index", new { id = id });
    }

    [HttpPost]
    public IActionResult ChangeStatus([FromRoute]string id,  Todo selected)
    {
        selected = _context.ToDos.Find(selected.Id);
        if(selected != null)
        { 
            if(selected.StatusId == "open")
            {
                selected.StatusId = "inprogress";
            }
            else
            {
                selected.StatusId = "closed";
            }
            _context.ToDos.Update(selected);
            _context.SaveChanges();
        }
        return RedirectToAction("Index", new { id = id });
    }

    
    [HttpPost]
    public IActionResult DeleteCompleted(string id)
    {
        var currentUser = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;
        var completed = _context.ToDos.Where(todo => todo.User.Id == currentUser.Id && todo.StatusId == "closed");
        if(completed != null) 
        {
            _context.ToDos.RemoveRange(completed);//if doesn't work, try foreach and remove one by one
            _context.SaveChanges();
        }
        return RedirectToAction("Index", new { id = id });
    }
}

