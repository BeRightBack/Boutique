using Boutique.Controllers;
using Boutique.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Boutique.Areas.template2.Controllers;

[Area("template2"), Authorize(Roles = "Administrator")]
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