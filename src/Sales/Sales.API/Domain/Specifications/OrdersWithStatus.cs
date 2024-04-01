using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Domain.Specifications;

public class OrdersWithStatus : BaseSpecification<Order>
{
    public OrdersWithStatus(OrderStatus status)
    {
        Criteria = order => order.Status == status;
    }
}