using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application;

public record PaymentDto(string? Id, int InvoiceId, PaymentStatus Status, string Currency, decimal Amount, DateTime DueDate, PaymentMethod PaymentMethod, string? Message = null);