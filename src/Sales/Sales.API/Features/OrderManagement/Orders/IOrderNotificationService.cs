using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.OrderManagement.Orders;

public interface IOrderNotificationService
{
    Task Created(int orderNo);

    Task Updated(int orderNo);

    Task Deleted(int orderNo);

    Task StatusUpdated(int orderNo, OrderStatusDto status);
}