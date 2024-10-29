
using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Invoicing.Domain.Events;
using YourBrand.Tenancy;

namespace YourBrand.Invoicing.Domain.Entities;

public class Invoice : AuditableEntity, IHasTenant, IHasOrganization
{
    readonly List<InvoiceItem> _items = new List<InvoiceItem>();

    private Invoice() { }

    public Invoice(InvoiceType type = InvoiceType.Invoice)
    {
        Id = Guid.NewGuid().ToString();

        Type = type;
        StatusId = 1;

        AddDomainEvent(new InvoiceCreated(Id));
    }

    public Invoice(DateTimeOffset? date, InvoiceType type = InvoiceType.Invoice, int status = 1, string currency = "SEK", string? note = null)
    {
        Id = Guid.NewGuid().ToString();

        IssueDate = date ?? DateTime.Now;
        Type = type;
        StatusId = status;
        Currency = currency;
        Notes = note;

        AddDomainEvent(new InvoiceCreated(Id));
    }

    public string Id { get; set; }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int? InvoiceNo { get; private set; }

    public DateTimeOffset? IssueDate { get; set; }

    public void SetIssueDate(DateTimeOffset? date)
    {
        if (IssueDate != date)
        {
            IssueDate = date;
            AddDomainEvent(new InvoiceDateChanged(Id, IssueDate));
        }
    }

    public InvoiceType Type { get; private set; }

    public void SetType(InvoiceType type)
    {
        if (Type != type)
        {
            Type = type;
            AddDomainEvent(new InvoiceTypeChanged(Id, Type));
        }
    }

    public InvoiceStatus Status { get; private set; }

    public int StatusId { get; set; }

    public DateTimeOffset? StatusDate { get; set; }

    public bool UpdateStatus(int status, TimeProvider timeProvider)
    {
        var oldStatus = StatusId;
        if (status != oldStatus)
        {
            if (oldStatus == 1)
            {
                IssueDate = timeProvider.GetUtcNow();
            }

            StatusId = status;
            StatusDate = timeProvider.GetUtcNow();

            //AddDomainEvent(new OrderUpdated(Id));
            AddDomainEvent(new InvoiceStatusUpdated(OrganizationId, Id, status, oldStatus));

            return true;
        }

        return false;
    }

    public DateTimeOffset? DeliveryDate { get; private set; }

    public void SetDeliveryDate(DateTimeOffset deliveryDate)
    {
        if (DeliveryDate != deliveryDate)
        {
            DeliveryDate = deliveryDate;
            //AddDomainEvent(new InvoiceDueDateChanged(Id, DeliveryDate));
        }
    }

    public DateTimeOffset? DueDate { get; private set; }

    public void SetDueDate(DateTimeOffset dueDate)
    {
        if (DueDate != dueDate)
        {
            DueDate = dueDate;
            AddDomainEvent(new InvoiceDueDateChanged(Id, DueDate));
        }
    }

    public Customer? Customer { get; set; }

    public string Currency { get; private set; }

    public void SetCurrency(string currency)
    {
        if (Currency != currency)
        {
            Currency = currency;
            //AddDomainEvent(new InvoiceDueDateChanged(Id, DueDate));
        }
    }

    public string? Reference { get; private set; }

    public void SetReference(string? reference)
    {
        if (Reference != reference)
        {
            Reference = reference;
            AddDomainEvent(new InvoiceReferenceChanged(Id, Reference));
        }
    }

    public bool VatIncluded { get; set; }

    public decimal SubTotal { get; private set; }

    public double? VatRate { get; set; }

    public decimal Vat { get; set; }

    public decimal Discount { get; set; }

    public List<InvoiceVatAmount> VatAmounts { get; set; } = new List<InvoiceVatAmount>();

    public bool Rounding { get; set; }

    public decimal? Rounded { get; set; }

    public decimal Total { get; private set; }

    public decimal? Paid { get; private set; }

    public BillingDetails? BillingDetails { get; set; }

    public ShippingDetails? ShippingDetails { get; set; }

    public void SetPaid(decimal amount)
    {
        if (Paid != amount)
        {
            Paid = amount;
            AddDomainEvent(new InvoiceAmountPaidChanged(Id, Paid));
        }

        /*
        if(Paid == Total) 
        {
            SetStatus(InvoiceStatus.Paid);
        }
        else if(Paid < Total) 
        {
            SetStatus(InvoiceStatus.PartiallyPaid);
        }
        else if(Paid > Total) 
        {
            SetStatus(InvoiceStatus.Overpaid);
        }
        */
    }

    public string? Notes { get; set; }

    public void SetNote(string note)
    {
        if (Notes != note)
        {
            Notes = note;
            AddDomainEvent(new InvoiceNoteChanged(Id, Notes));
        }
    }

    public IReadOnlyList<InvoiceItem> Items => _items;

    public InvoiceItem AddItem(
        ProductType productType,
        string description,
        string? productId,
        decimal unitPrice,
        string unit,
        decimal? discount,
        double vatRate,
        double quantity)
    {
        var invoiceItem = new InvoiceItem(this, productType, description, productId, unitPrice, unit, discount, vatRate, quantity);
        invoiceItem.OrganizationId = OrganizationId;
        _items.Add(invoiceItem);

        Update();

        return invoiceItem;
    }

    public void DeleteItem(InvoiceItem item)
    {
        if (Status.Id != (int)Enums.InvoiceStatus.Draft)
        {
            throw new Exception();
        }

        _items.Remove(item);

        Update();
    }

    public void Update()
    {
        UpdateVatAmounts();

        Vat = Items.Sum(x => x.Vat.GetValueOrDefault());
        Total = Items.Sum(x => x.Total);
        SubTotal = Total - Vat;
        Discount = Items.Sum(x => (decimal)x.Quantity * x.Discount.GetValueOrDefault());

        Rounded = null;
        if (Rounding)
        {
            Rounded = Math.Round(0m, MidpointRounding.AwayFromZero);
        }

        Total -= DomesticService?.RequestedAmount ?? 0;
    }

    private void UpdateVatAmounts()
    {
        VatAmounts.ForEach(x =>
        {
            x.Vat = 0;
            x.SubTotal = 0;
            x.Total = 0;
        });

        foreach (var item in Items)
        {
            item.Update();

            var vatAmount = VatAmounts.FirstOrDefault(x => x.VatRate == item.VatRate);
            if (vatAmount is null)
            {
                vatAmount = new InvoiceVatAmount()
                {
                    VatRate = item.VatRate.GetValueOrDefault(),
                    Name = $"{item.VatRate * 100}%"
                };

                VatAmounts.Add(vatAmount);
            }

            vatAmount.Vat += item.Vat.GetValueOrDefault();
            vatAmount.SubTotal += item.Total - item.Vat.GetValueOrDefault();
            vatAmount.Total += item.Total;
        }

        VatAmounts.ToList().ForEach(x =>
        {
            if (x.Vat == 0 && x.VatRate != 0)
            {
                VatAmounts.Remove(x);
            }
        });

        if (VatAmounts.Count == 1)
        {
            var vatAmount = VatAmounts.First();

            VatRate = vatAmount.VatRate;
        }
        else
        {
            VatRate = null;
        }
    }

    public async Task AssignInvoiceNo(InvoiceNumberFetcher invoiceNumberFetcher, CancellationToken cancellationToken = default)
    {
        if(InvoiceNo is not null)
        {
            throw new InvalidOperationException("Invoice number already set");
        }

        InvoiceNo = await invoiceNumberFetcher.GetNextNumberAsync(OrganizationId, cancellationToken);
    }

    public InvoiceDomesticService? DomesticService { get; set; }
}

[Index(nameof(Id), nameof(CustomerNo))]
public class Customer
{
    public string Id { get; set; }

    public long CustomerNo { get; set; }

    public string Name { get; set; }
}

public class InvoiceVatAmount
{
    public double VatRate { get; set; }

    public string Name { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}

public record InvoiceDomesticService(
    Domain.Entities.DomesticServiceKind Kind,
    string Buyer,
    string Description,
    decimal RequestedAmount)
{
    public PropertyDetails? PropertyDetails { get; set; }
};

public record PropertyDetails(PropertyType Type, string? PropertyDesignation, string? ApartmentNo, string? OrganizationNo);