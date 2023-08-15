
using System;
using System.Collections.Generic;
using Boutique.Entity;

namespace Boutique.Services;
public interface IOrderCountService
{
    IList<OrderCount> GetAllOrderCount();

    IList<OrderCount> GetAllOrderCount(int take);

    OrderCount GetOrderCountByDate(DateTime date);

    void InsertOrderCount(OrderCount orderCount);

    void UpdateOrderCount(OrderCount orderCount);
}
