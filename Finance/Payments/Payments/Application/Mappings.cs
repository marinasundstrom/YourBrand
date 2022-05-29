using System;

using YourBrand.Payments.Domain.Entities;

namespace YourBrand.Payments.Application;

public static class Mapper
{
    public static PaymentDto ToDto(this Payment payment) => new PaymentDto(payment.Id, payment.InvoiceId, payment.Status, payment.Currency, payment.Amount, payment.DueDate, payment.PaymentMethod, payment.Reference, payment.Message, payment.AmountCaptured);
}

