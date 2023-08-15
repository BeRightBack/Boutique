using Boutique.Controllers;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class TestController : BaseController
{
    public TestController(
        ILanguageService languageService,
        ILocalizationService localizationService) : base(languageService, localizationService)
    { }

    public IActionResult Index()
    {
        ViewData["Something"] = Localize("customer.page.create.title");
        return View();
    }
}