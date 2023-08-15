
using System;
using System.Collections.Generic;
using Boutique.Entity;

namespace Boutique.Services;

public interface IVisitorCountService
{
    IList<VisitorCount> GetAllVisitorCount();

    IList<VisitorCount> GetAllVisitorCount(int take);

    VisitorCount GetVisitorCountByDate(DateTime date);

    void InsertVisitorCount(VisitorCount visitorCount);

    void UpdateVisitorCount(VisitorCount visitorCount);
}
