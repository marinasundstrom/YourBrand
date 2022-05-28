using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Hubs;

public interface IPaymentsHubClient
{
    Task PaymentStatusUpdated(string id, PaymentStatus Status);

    Task PaymentInvoiceIdUpdated(string id, int? invoiceId);
}