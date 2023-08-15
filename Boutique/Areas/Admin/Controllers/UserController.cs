using System.Threading.Tasks;
using Boutique.Areas.Admin.Models;
using Boutique.Controllers;
using Boutique.Entity;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class UserController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserValidator<ApplicationUser> _userValidator;
    private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;


    public UserController(
        ILanguageService languageService,
        ILocalizationService localizationService,
        UserManager<ApplicationUser> userManager,
        IUserValidator<ApplicationUser> userValidator,
        IPasswordValidator<ApplicationUser> passwordValidator,
        IPasswordHasher<ApplicationUser> passwordHasher) : base(languageService, localizationService)
    {
        _userManager = userManager;
        _userValidator = userValidator;
        _passwordValidator = passwordValidator;
        _passwordHasher = passwordHasher;
    }

    public ViewResult Index() => View(_userManager.Users);

    public async Task<IActionResult> Details(string id)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            return View(user);
        }
        else
        {
            return RedirectToAction("Index");
        }
    }
    public ViewResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(CreateModel model)
    {
        if (ModelState.IsValid)
        {
            ApplicationUser user = new()
            {
                UserName = model.Name,
                Email = model.Email
            };
            IdentityResult result
                = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                AddErrorsFromResult(result);
            }
        }
        else
        {
            ModelState.AddModelError("", "User Not Found");
        }
        return View("Index", _userManager.Users);
    }

    public async Task<IActionResult> Edit(string id)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            return View(user);
        }
        else
        {
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, string email,
            string password)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            user.Email = email;
            IdentityResult validEmail
                = await _userValidator.ValidateAsync(_userManager, user);
            if (!validEmail.Succeeded)
            {
                AddErrorsFromResult(validEmail);
            }
            IdentityResult validPass = null;
            if (!string.IsNullOrEmpty(password))
            {
                validPass = await _passwordValidator.ValidateAsync(_userManager, user, password);
                if (validPass.Succeeded)
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, password);
                }
                else
                {
                    AddErrorsFromResult(validPass);
                }
            }
            if ((validEmail.Succeeded && validPass == null) || (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
            {
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
        }
        else
        {
            ModelState.AddModelError("", "User Not Found");
        }
        return View(user);
    }

    private void AddErrorsFromResult(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }
}
