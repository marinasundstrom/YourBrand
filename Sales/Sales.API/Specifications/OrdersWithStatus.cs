using System;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.API.Features.OrderManagement.Domain.Specifications;

public class OrdersWithStatus : BaseSpecification<Order>
{
    public OrdersWithStatus(OrderStatus status)
    {
        Criteria = order => order.Status == status;
    }
}