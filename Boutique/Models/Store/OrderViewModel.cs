using System;
using System.Collections.Generic;
using Boutique.Entity;

namespace Boutique.Models;

public class OrderViewModel
{
    public Guid UserId { get; set; }
    public required string OrderNumber { get; set; }
    public Guid BillingAddressId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderPlacedDateTime { get; set; }
    public DateTime OrderCompletedDateTime { get; set; }
    public decimal TotalOrderPrice { get; set; }

    public BillingAddressModel BillingAddress { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class BillingAddressModel
{
}
