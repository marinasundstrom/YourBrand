using Microsoft.AspNetCore.SignalR;

using YourBrand.Payments.Domain.Enums;
using YourBrand.Payments.Hubs;

namespace YourBrand.Payments.Application.Services;

public class PaymentsHubClient(IHubContext<PaymentsHub, IPaymentsHubClient> hubContext) : IPaymentsHubClient
{
    public async Task PaymentStatusUpdated(string id, PaymentStatus status)
    {
        await hubContext.Clients.Group($"payment-{id}").PaymentStatusUpdated(id, status);
        //await _hubContext.Clients.All.PaymentStatusUpdated(id, status);
    }
}