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
        await _hubContext.Clients.All.PaymentStatusUpdated(id, status);
    }

    public async Task PaymentInvoiceIdUpdated(string id, int? invoiceId)
    {
        await _hubContext.Clients.All.PaymentInvoiceIdUpdated(id, invoiceId);
    }
}