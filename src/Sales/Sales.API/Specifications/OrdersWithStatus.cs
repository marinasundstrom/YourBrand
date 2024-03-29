using System;

using YourBrand.Sales.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.Features.OrderManagement.Domain.Specifications;

public class OrdersWithStatus : BaseSpecification<Order>
{
    public OrdersWithStatus(OrderStatus status)
    {
        Criteria = order => order.Status == status;
    }
}