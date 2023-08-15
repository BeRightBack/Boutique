
using Boutique.Entity;
using System;
using System.Collections.Generic;

namespace Boutique.Services;

public interface IOrderService
{
    IList<Order> GetAllOrders();

    Order GetOrderById(Guid id);

    Order GetOrderByOrderId(string id);

    IList<Order> GetAllOrdersByUserId(Guid userId);

    void InsertOrder(Order order);

    void UpdateOrder(Order order);

    void DeleteOrders(IList<Guid> ids);
}
