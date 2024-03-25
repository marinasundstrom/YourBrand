using System;
using System.Collections.Generic;

using YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.Subscriptions;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public SubscriptionPlanShortDto Plan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get;  set; }
    public OrderShortDto? Order { get;  set; }
    public string? OrderItemId { get; set; }
}

public class SubscriptionPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ProductId { get; set; }
    public decimal? Price { get; set; }
}

public class SubscriptionPlanShortDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ProductId { get; set; }
    public decimal? Price { get; set; }
}