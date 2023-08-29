using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Boutique.Services;
using Boutique.Controllers;

namespace Boutique.Areas.template3.Controllers
{
    [Area("template3"), Authorize(Roles = "Administrator")]
    public class AdminController :  BaseController 
    {

        public AdminController(
            ILanguageService languageService,
            ILocalizationService localizationService) : base(languageService, localizationService)
        {  }

       public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard", new { area = "template3" });
        }
    }

    
}
