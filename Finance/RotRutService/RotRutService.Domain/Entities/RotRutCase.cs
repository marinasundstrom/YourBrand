using System;

using YourBrand.RotRutService.Domain.Common;
using YourBrand.RotRutService.Domain.Enums;

namespace YourBrand.RotRutService.Domain.Entities;

public class RotRutCase : AuditableEntity
{
    private RotRutCase()
    {
    }

    public RotRutCase(DomesticServiceKind kind, string buyer, DateTime paymentDate, decimal laborCost, decimal paidAmount, decimal requestedAmount, int invoiceId, decimal otherCosts, double hours, decimal materialCost, decimal? receivedAmount)
    {
        Kind = kind;
        Status = RotRutCaseStatus.Created;
        Buyer = buyer;
        PaymentDate = paymentDate;
        LaborCost = laborCost;
        PaidAmount = paidAmount;
        RequestedAmount = requestedAmount;
        InvoiceNo = invoiceId;
        OtherCosts = otherCosts;
        Hours = hours;
        MaterialCost = materialCost; 
        ReceivedAmount = receivedAmount;
    }

    public int Id { get; private set; }

    public RotRutRequest? Request { get; private set; }

    public DomesticServiceKind Kind { get; private set; }

    public RotRutCaseStatus Status { get; set; }
    
    public string Buyer { get; private set; }

    public DateTime PaymentDate { get; private set; }
    
    public decimal LaborCost { get; private set; }

    public decimal PaidAmount { get; private set; }

    public decimal RequestedAmount { get; private set; }
    
    public int InvoiceNo { get; private set; }
    
    public decimal OtherCosts { get; private set; }

    public double Hours { get; private set; }

    public decimal MaterialCost { get; private set; }

    public decimal? ReceivedAmount { get; set; }

    public Rot? Rot { get; set; }

    public Rut? Rut { get; set; }

}

public class Rot
{
    public HomeRepairAndMaintenanceServiceType? ServiceType { get; set; }

    public string? PropertyDesignation { get; set; }

    public string? ApartmentNo { get; set; }

    public string? OrganizationNo { get; set; }
}

public class Rut
{
    public HouseholdServiceType? ServiceType { get; set; }
}

public enum RotRutCaseStatus
{
    Created,
    InvoicePaid,
    RequestSent,
    RequestConfirmed
}