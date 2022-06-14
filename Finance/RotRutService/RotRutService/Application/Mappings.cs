using YourBrand.RotRutService.Domain.Entities;
using YourBrand.RotRutService.Domain.Enums;

namespace YourBrand.RotRutService.Application;

public static class Mappings 
{
    public static RotRutCaseDto ToDto(this RotRutCase invoice) 
    {
        return new RotRutCaseDto((DomesticServiceKind)invoice.Kind, invoice.Status, invoice.Buyer,
            invoice.PaymentDate, invoice.LaborCost, invoice.PaidAmount,
            invoice.RequestedAmount, invoice.InvoiceId, invoice.OtherCosts,
            invoice.Hours, invoice.MaterialCost, invoice.ReceivedAmount);
    }
}

public record RotRutCaseDto(
    DomesticServiceKind Kind, RotRutCaseStatus Status, string Buyer, DateTime PaymentDate,
    decimal LaborCost, decimal PaidAmount, decimal RequestedAmount,
    int InvoiceId, decimal OtherCosts, double Hours, decimal MaterialCost,
    decimal? ReceivedAmount);