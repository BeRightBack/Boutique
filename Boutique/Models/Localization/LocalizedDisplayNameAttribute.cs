using System.ComponentModel;
using System.Threading;
using Boutique.Entity;
using Boutique.Services;
using Microsoft.AspNetCore.Http;

namespace Boutique.Models;
public class LocalizedDisplayNameAttribute : DisplayNameAttribute
{
    private readonly string _resourceKey = string.Empty;
    public static HttpContext HttpContext => new HttpContextAccessor().HttpContext;

    public LocalizedDisplayNameAttribute(string resourceKey) : base(resourceKey)
    {
        _resourceKey = resourceKey;
    }

    public override string DisplayName
    {
        get
        {
            // Resolve Services
            ILanguageService _languageService = (ILanguageService)HttpContext.RequestServices.GetService(typeof(ILanguageService));
            ILocalizationService _localizationService = (ILocalizationService)HttpContext.RequestServices.GetService(typeof(ILocalizationService));

            // Get Language Information from Database based on Current Culture
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            var language = _languageService.GetLanguageByCulture(currentCulture);

            if (language != null)
            {
                // Get String Resource Value from Database
                var stringResource = _localizationService.GetStringResource(_resourceKey, language.Id);

                if (stringResource != null && !string.IsNullOrEmpty(stringResource.Value))
                {
                    return stringResource?.Value ?? _resourceKey;
                }
                if (stringResource == null || string.IsNullOrEmpty(stringResource.Value))
                {
                    var _resource_fr = new StringResource
                    {
                        LanguageId = 1,
                        Name = _resourceKey,
                        Value = _resourceKey + "-fr"
                    };
                    var _resource_en = new StringResource
                    {
                        LanguageId = 2,
                        Name = _resourceKey,
                        Value = _resourceKey + "-en"
                    };
                    var _resource_es = new StringResource
                    {
                        LanguageId = 3,
                        Name = _resourceKey,
                        Value = _resourceKey + "-es"
                    };
                    _localizationService.AddOrUpdateStringResource(_resource_fr);
                    _localizationService.AddOrUpdateStringResource(_resource_en);
                    _localizationService.AddOrUpdateStringResource(_resource_es);
                    return _resourceKey;
                }
            }

            return _resourceKey;
        }
    }
}