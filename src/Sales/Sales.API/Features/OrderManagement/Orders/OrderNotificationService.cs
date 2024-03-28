using Microsoft.AspNetCore.SignalR;

using YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;

using YourBrand.Sales.Features.Services;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders;

public class OrderNotificationService : IOrderNotificationService
{
    private readonly IHubContext<OrdersHub, IOrdersHubClient> hubsContext;

    public OrderNotificationService(IHubContext<OrdersHub, IOrdersHubClient> hubsContext)
    {
        this.hubsContext = hubsContext;
    }

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