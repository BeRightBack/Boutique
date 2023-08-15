using System;
using System.Collections.Generic;

namespace Boutique.Entity;

public enum OrderStatus
{
    Pending,
    Processing,
    Complete,
    Cancelled
};

public class Order
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; }
    public Guid UserId { get; set; }
    public Guid BillingAddressId { get; set; }
    public decimal TotalOrderPrice { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderPlacementDateTime { get; set; }
    public DateTime? OrderCompletedDateTime { get; set; }

    public virtual ICollection<OrderItem> Items { get; set; }
}
