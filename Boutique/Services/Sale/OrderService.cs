using Boutique.EFRepository;
using Boutique.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;
using Boutique.Data;

namespace Boutique.Services;
public class OrderService : IOrderService
{
    private readonly CatalogDbContext context;
    private readonly IOrderCountService orderCountService;
    private readonly IRepository<Order> orderRepository;
    private readonly IRepository<OrderItem> orderItemRepository;

    public OrderService(
        CatalogDbContext context,
        IOrderCountService orderCountService,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository)
    {
        this.context = context;
        this.orderCountService = orderCountService;
        this.orderRepository = orderRepository;
        this.orderItemRepository = orderItemRepository;
    }

    public IList<Order> GetAllOrders()
    {
        // TODO: update when lazy loading is available
        var entities = context.Orders
            .Include(x => x.Items)
            .AsNoTracking()
            .ToList();

        return entities;
    }

    public Order GetOrderById(Guid id)
    {
        // TODO: update when lazy loading is available
        return context.Orders
            .Include(x => x.Items)
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == id);
    }

    public Order GetOrderByOrderId(string id)
    {
        // TODO: update when lazy loading is available
        return context.Orders
            .Include(x => x.Items)
            .AsNoTracking()
            .SingleOrDefault(x => x.OrderNumber == id);
    }

    public IList<Order> GetAllOrdersByUserId(Guid userId)
    {
        // TODO: update when lazy loading is available
        var entities = context.Orders
            .Include(x => x.Items)
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToList();

        return entities;
    }

    public void InsertOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        orderRepository.Insert(order);
        orderRepository.SaveChanges();

        // add or update order count
        var orderCountEntity = orderCountService.GetOrderCountByDate(DateTime.Now);
        if (orderCountEntity != null)
            orderCountService.UpdateOrderCount(orderCountEntity);
        else
        {
            var orderCountModel = new OrderCount
            {
                Date = DateTime.Now,
                Count = 1
            };
            orderCountService.InsertOrderCount(orderCountModel);
        }
    }

    public void UpdateOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        orderRepository.Update(order);
        orderRepository.SaveChanges();
    }

    public void DeleteOrders(IList<Guid> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        foreach (var id in ids)
            orderRepository.Delete(GetOrderById(id));

        orderRepository.SaveChanges();
    }
}
