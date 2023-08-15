using Boutique.Areas.Admin.Models;
using Boutique.Controllers;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class DocumentController : BaseController
{
    private readonly ProtectedDocument[] docs = new ProtectedDocument[] {
        new ProtectedDocument { Title = "Q3 Budget", Author = "Alice",
            Editor = "Joe"},
        new ProtectedDocument { Title = "Project Plan", Author = "Bob",
            Editor = "Alice"}
    };

    private readonly IAuthorizationService _authorizationService;

    public DocumentController(
        ILanguageService languageService,
        ILocalizationService localizationService,
        IAuthorizationService authorizationService) : base(languageService, localizationService)
    {
        _authorizationService = authorizationService;
    }

    public ViewResult Index() => View(docs);

    public async Task<IActionResult> Edit(string title)
    {
        ProtectedDocument doc = docs.FirstOrDefault(d => d.Title == title);
        AuthorizationResult authorized = await _authorizationService.AuthorizeAsync(User,
            doc, "AuthorsAndEditors");
        if (authorized.Succeeded)
        {
            return View("Index", doc);
        }
        else
        {
            return new ChallengeResult();
        }
    }
}