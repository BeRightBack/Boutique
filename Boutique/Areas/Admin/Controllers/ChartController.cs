using Boutique.Controllers;
using Boutique.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Administrator")]
public class ChartController : BaseController
{
    private readonly IOrderCountService _orderCountService;
    private readonly IVisitorCountService _visitorCountService;

    public ChartController(
        ILanguageService languageService,
        ILocalizationService localizationService,
        IOrderCountService orderCountService,
        IVisitorCountService visitorCountService) : base( languageService, localizationService)
    {
        _orderCountService = orderCountService;
        _visitorCountService = visitorCountService;
    }

    public IActionResult GetVisitorCount()
    {
        var results = _visitorCountService.GetAllVisitorCount(7)
            .OrderBy(x => x.Date);

        return Json(results);
    }

    public IActionResult GetOrderCount()
    {
        var results = _orderCountService.GetAllOrderCount(7)
            .OrderBy(x => x.Date);

        return Json(results);
    }
}