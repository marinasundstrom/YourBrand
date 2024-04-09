using Microsoft.AspNetCore.SignalR;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.OrderManagement.Orders;

public class OrderNotificationService(IHubContext<OrdersHub, IOrdersHubClient> hubsContext) : IOrderNotificationService
{
    public async Task Created(int orderNo)
    {
        await hubsContext.Clients.All.Created(orderNo);
    }

    public async Task Updated(int orderNo)
    {
        await hubsContext.Clients.All.Updated(orderNo);
    }

    public async Task Deleted(int orderNo)
    {
        await hubsContext.Clients.All.Deleted(orderNo);
    }

    public async Task StatusUpdated(int orderNo, OrderStatusDto status)
    {
        await hubsContext.Clients.All.StatusUpdated(orderNo, status);
    }
}