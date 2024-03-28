using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace YourBrand.Analytics.Application.Hubs;

[Authorize]
public sealed class TodosHub : Hub<ITodosHubClient>
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
