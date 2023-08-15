
using System;
using System.Collections.Generic;
using Boutique.EFRepository;
using Boutique.Entity;
using System.Linq;

namespace Boutique.Services;

public class OrderCountService : IOrderCountService
{

    private readonly IRepository<OrderCount> orderCountRepository;

    public OrderCountService(IRepository<OrderCount> orderCountRepository)
    {
        this.orderCountRepository = orderCountRepository;
    }

    public IList<OrderCount> GetAllOrderCount()
    {
        return orderCountRepository.GetAll().ToList();
    }

    public IList<OrderCount> GetAllOrderCount(int take)
    {
        return orderCountRepository.GetAll().Take(take).ToList();
    }

    public OrderCount GetOrderCountByDate(DateTime date)
    {
        return orderCountRepository.FindByExpression(x => x.Date == date.Date);
    }

    public void InsertOrderCount(OrderCount orderCount)
    {
        if (orderCount == null)
            throw new ArgumentNullException(nameof(orderCount));

        orderCount.Date = orderCount.Date.Date;

        orderCountRepository.Insert(orderCount);
        orderCountRepository.SaveChanges();
    }

    public void UpdateOrderCount(OrderCount orderCount)
    {
        if (orderCount == null)
            throw new ArgumentNullException(nameof(orderCount));

        orderCount.Date = orderCount.Date.Date;
        orderCount.Count++;

        orderCountRepository.Update(orderCount);
        orderCountRepository.SaveChanges();
    }
}
