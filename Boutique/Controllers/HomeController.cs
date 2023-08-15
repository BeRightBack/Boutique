using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Boutique.Models;
using Boutique.Services;

namespace Boutique.Controllers;

public class HomeController : BaseController
{

    public HomeController(
        ILanguageService languageService,
        ILocalizationService localizationService) : base(languageService, localizationService)
    {

    }

    public IActionResult Index()
    {
        return View();
    }

}
