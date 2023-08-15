using System.Diagnostics;
using Boutique.Entity;
using Boutique.Models;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Boutique.Controllers;

public class BaseController : Controller
{
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    public BaseController(ILanguageService languageService, ILocalizationService localizationService)
    {
        _languageService = languageService;
        _localizationService = localizationService;
    }

    public HtmlString Localize(string resourceKey, params object[] args)
    {
        var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

        var language = _languageService.GetLanguageByCulture(currentCulture);
        if (language != null)
        {
            var stringResource = _localizationService.GetStringResource(resourceKey, language.Id);
            if (stringResource == null || string.IsNullOrEmpty(stringResource.Value))
            {
                var _resource_fr = new StringResource
                {
                    LanguageId = 1,
                    Name = resourceKey,
                    Value = resourceKey + "-fr"
                };
                var _resource_en = new StringResource
                {
                    LanguageId = 2,
                    Name = resourceKey,
                    Value = resourceKey + "-en"
                };
                var _resource_es = new StringResource
                {
                    LanguageId = 3,
                    Name = resourceKey,
                    Value = resourceKey + "-es"
                };
                _localizationService.AddOrUpdateStringResource(_resource_fr);
                _localizationService.AddOrUpdateStringResource(_resource_en);
                _localizationService.AddOrUpdateStringResource(_resource_es);
                return new HtmlString(resourceKey);
            }

            return new HtmlString((args == null || args.Length == 0)
                ? stringResource.Value
                : string.Format(stringResource.Value, args));
        }
        return new HtmlString(resourceKey);
    }

    public String LocString(string resourceKey, params object[] args)
    {
        var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

        var language = _languageService.GetLanguageByCulture(currentCulture);
        if (language != null)
        {
            var stringResource = _localizationService.GetStringResource(resourceKey, language.Id);
            if (stringResource == null || string.IsNullOrEmpty(stringResource.Value))
            {
                var _resource_fr = new StringResource
                {
                    LanguageId = 1,
                    Name = resourceKey,
                    Value = resourceKey + "-fr"
                };
                var _resource_en = new StringResource
                {
                    LanguageId = 2,
                    Name = resourceKey,
                    Value = resourceKey + "-en"
                };
                var _resource_es = new StringResource
                {
                    LanguageId = 3,
                    Name = resourceKey,
                    Value = resourceKey + "-es"
                };
                _localizationService.AddOrUpdateStringResource(_resource_fr);
                _localizationService.AddOrUpdateStringResource(_resource_en);
                _localizationService.AddOrUpdateStringResource(_resource_es);
                return new String(resourceKey);
            }

            return new String((args == null || args.Length == 0)
                ? stringResource.Value
                : string.Format(stringResource.Value, args));
        }
        return new String(resourceKey);
    }

    [HttpPost]
    public IActionResult ChangeLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            }
        );

        return LocalRedirect(returnUrl);
    }

    [HttpPost]
    public IActionResult SetCulture(string culture, string redirectUri)
    {
        Log.Information("SetCulture");
        if (culture != null)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), Secure = true, SameSite = SameSiteMode.None });
        }

        return LocalRedirect(redirectUri);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }
}
