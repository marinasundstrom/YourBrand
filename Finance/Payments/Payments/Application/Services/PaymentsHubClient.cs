using Microsoft.AspNetCore.SignalR;

using YourBrand.Payments.Domain.Enums;
using YourBrand.Payments.Hubs;

namespace YourBrand.Payments.Application.Services;

public class PaymentsHubClient : IPaymentsHubClient
{
    private readonly IHubContext<PaymentsHub, IPaymentsHubClient> _hubContext;

    public PaymentsHubClient(IHubContext<PaymentsHub, IPaymentsHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PaymentStatusUpdated(string id, PaymentStatus status)
    {
        await _hubContext.Clients.Group($"payment-{id}").PaymentStatusUpdated(id, status);
    }
}