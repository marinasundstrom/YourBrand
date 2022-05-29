using Microsoft.AspNetCore.SignalR;

namespace YourBrand.Payments.Hubs;

public class PaymentsHub : Hub<IPaymentsHubClient>
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        var httpContext = Context.GetHttpContext();

        if (httpContext is not null)
        {
            if (httpContext.Request.Query.TryGetValue("paymentId", out var paymentId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"paymentId-{paymentId}");
            }
        }
    }
}
