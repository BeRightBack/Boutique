using System;
using System.Threading.Tasks;
using Boutique.Entity;
using Boutique.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Boutique.Middleware;

public class VisitorCounterMiddleware
{
    private readonly RequestDelegate next;
    private readonly IVisitorCountService visitorCounterService;

    public VisitorCounterMiddleware(
        RequestDelegate next,
        IVisitorCountService visitorCounterService)
    {
        this.next = next;
        this.visitorCounterService = visitorCounterService;
    }

    public Task Invoke(HttpContext context)
    {
        if (context.Session.GetString("visitor_counter") == null || context.Session.GetString("visitor_counter") != "recorder")
        {
            context.Session.SetString("visitor_counter", "recorder");
            var visitorCountEntity = visitorCounterService.GetVisitorCountByDate(DateTime.Now);
            if (visitorCountEntity != null)
            {
                visitorCounterService.UpdateVisitorCount(visitorCountEntity);
            }
            else
            {
                var visitorModel = new VisitorCount
                {
                    Date = DateTime.Now,
                    ViewCount = 1
                };
                visitorCounterService.InsertVisitorCount(visitorModel);
            }
        }

        return next(context);
    }
}

public static class VisitorCounterMiddlewareExtentions
{
    public static IApplicationBuilder UseVisitorCounter(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<VisitorCounterMiddleware>();
    }
}
