using Boutique.Controllers;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boutique.Areas.Test.Controllers
{
    [Area("Test"), Authorize(Roles = "Administrator")]
    public class ImageController : BaseController
    {
        public ImageController(
            ILanguageService languageService,
            ILocalizationService localizationService) : base(languageService, localizationService)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
