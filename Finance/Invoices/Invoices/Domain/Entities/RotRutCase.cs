using System;

using YourBrand.Invoices.Domain.Common;

namespace YourBrand.Invoices.Domain.Entities;

public class RotRutCase : AuditableEntity
{
    private RotRutCase()
    {
    }

    public RotRutCase(DomesticServiceKind kind, int invoiceId, string buyer, decimal total, double hours, decimal laborCost, decimal materialCost, decimal otherCosts, decimal requestedAmount, decimal? receivedAmount)
    {
        Kind = kind;
        Status = RotRutCaseStatus.Created;
        InvoiceId = invoiceId;
        Buyer = buyer;
        Total = total;
        Hours = hours;
        LaborCost = laborCost;
        MaterialCost = materialCost;
        OtherCosts = otherCosts;
        RequestedAmount = requestedAmount;
        ReceivedAmount = receivedAmount;
    }

    public int Id { get; private set; }

    public RotRutRequest? Request { get; private set; }

    public DomesticServiceKind Kind { get; private set; }

    public RotRutCaseStatus Status { get; set; }

    public int InvoiceId { get; private set; }

    public string Buyer { get; private set; }

    public decimal Total { get; private set; }

    public double Hours { get; private set; }

    public decimal LaborCost { get; private set; }

    public decimal MaterialCost { get; private set; }

    public decimal MaterialCosts { get; private set; }

    public decimal OtherCosts { get; private set; }

    public decimal RequestedAmount { get; private set; }

    public decimal? ReceivedAmount { get; private set; }

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