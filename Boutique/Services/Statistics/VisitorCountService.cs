
using Boutique.Data;
using Boutique.EFRepository;
using Boutique.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Boutique.Services;
public class VisitorCountService : IVisitorCountService
{
    private readonly CatalogDbContext context;
    private readonly IRepository<VisitorCount> visitorCountRepository;

    public VisitorCountService(
        CatalogDbContext context,
        IRepository<VisitorCount> visitorCountRepository)
    {
        this.context = context;
        this.visitorCountRepository = visitorCountRepository;
    }

    public IList<VisitorCount> GetAllVisitorCount()
    {
        return visitorCountRepository.GetAll().ToList();
    }

    public IList<VisitorCount> GetAllVisitorCount(int take)
    {
        return visitorCountRepository.GetAll().Take(take).ToList(); ;
    }

    public VisitorCount GetVisitorCountByDate(DateTime date)
    {
        //return _visitorCountRepository.FindByExpression(x => x.Date == date.Date);
        return context.VisitorCounts.SingleOrDefault(x => x.Date == date.Date);
    }

    public void InsertVisitorCount(VisitorCount visitorCount)
    {
        if(visitorCount == null)
            throw new ArgumentNullException(nameof(visitorCount));

        visitorCount.Date = visitorCount.Date.Date;

        visitorCountRepository.Insert(visitorCount);
        visitorCountRepository.SaveChanges();
    }

    public void UpdateVisitorCount(VisitorCount visitorCount)
    {
        if (visitorCount == null)
            throw new ArgumentNullException(nameof(visitorCount));

        visitorCount.Date = visitorCount.Date.Date;
        visitorCount.ViewCount++;

        visitorCountRepository.Update(visitorCount);
        visitorCountRepository.SaveChanges();
    }
}
