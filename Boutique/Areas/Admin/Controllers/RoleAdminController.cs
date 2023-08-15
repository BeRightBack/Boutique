using Boutique.Areas.Admin.Models;
using Boutique.Controllers;
using Boutique.Entity;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class RoleAdminController : BaseController
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleAdminController(
        ILanguageService languageService,
        ILocalizationService localizationService,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager) : base(languageService, localizationService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public ViewResult Index() => View(_roleManager.Roles);

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create([Required] string name)
    {
        if (ModelState.IsValid)
        {
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                AddErrorsFromResult(result);
            }
        }
        return View(name);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        IdentityRole role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            IdentityResult result = await _roleManager.DeleteAsync(role);
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
            ModelState.AddModelError("", "No role found");
        }
        return View("Index", _roleManager.Roles);
    }

    public async Task<IActionResult> Edit(string id)
    {

        IdentityRole role = await _roleManager.FindByIdAsync(id);
        List<ApplicationUser> members = new();
        List<ApplicationUser> nonMembers = new();
        foreach (ApplicationUser user in _userManager.Users)
        {
            var list = await _userManager.IsInRoleAsync(user, role.Name)
                ? members : nonMembers;
            list.Add(user);
        }
        return View(new RoleEditModel
        {
            Role = role,
            Members = members,
            NonMembers = nonMembers
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(RoleModificationModel model)
    {
        IdentityResult result;
        if (ModelState.IsValid)
        {
            foreach (string userId in model.IdsToAdd ?? Array.Empty<string>())
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await _userManager.AddToRoleAsync(user,
                        model.RoleName);
                    if (!result.Succeeded)
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            foreach (string userId in model.IdsToDelete ?? Array.Empty<string>())
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await _userManager.RemoveFromRoleAsync(user,
                        model.RoleName);
                    if (!result.Succeeded)
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
        }

        if (ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return await Edit(model.RoleId);
        }
    }

    private void AddErrorsFromResult(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }
}
