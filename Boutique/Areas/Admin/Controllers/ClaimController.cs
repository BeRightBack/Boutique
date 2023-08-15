using Boutique.Controllers;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class ClaimController : BaseController
{
    public ClaimController(
        ILanguageService languageService,
        ILocalizationService localizationService) : base(languageService, localizationService)
    { }

    public ViewResult Index() => View(User?.Claims);
}
