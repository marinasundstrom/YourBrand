using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders;

public interface IOrdersHub
{

}

[Authorize]
public sealed class OrdersHub : Hub<IOrdersHubClient>, IOrdersHub
{
    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is not null)
        {
            if (httpContext.Request.Query.TryGetValue("itemId", out var itemId))
            {
                Groups.AddToGroupAsync(this.Context.ConnectionId, $"item-{itemId}");
            }
        }

        return base.OnConnectedAsync();
    }
}