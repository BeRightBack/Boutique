using Boutique.Controllers;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boutique.Areas.template.Controllers;

[Area("template"), Authorize(Roles = "Administrator")]
public class DashboardController : BaseController
{
    public DashboardController(
        ILanguageService languageService,
        ILocalizationService localizationService) : base(languageService, localizationService)
    { }

    public IActionResult Index()
    {
        return View();
    }
}