using System;

using Invoices.Domain.Common;
using Invoices.Domain.Enums;
using Invoices.Domain.Events;

namespace Invoices.Domain.Entities;

public class Invoice : IHasDomainEvents
{
    readonly List<InvoiceItem> _items = new List<InvoiceItem>();

    private Invoice() { }

    public Invoice(DateTime? date, InvoiceType type = InvoiceType.Invoice, InvoiceStatus status = InvoiceStatus.Draft, string? note = null)
    {
        Date = date ?? DateTime.Now;
        Type = type;
        Status = status;
        Note = note;

        DomainEvents.Add(new InvoiceCreated(Id));
    }

    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public void SetDate(DateTime? date)
    {
        if(Date != date) 
        {
            Date = date;
            DomainEvents.Add(new InvoiceDateChanged(Id, Date));
        }
    }

    public InvoiceType Type { get; private set; }

    public void SetType(InvoiceType type)
    {
        if(Type != type) 
        {
            Type = type;
            DomainEvents.Add(new InvoiceTypeChanged(Id, Type));
        }
    }

    public InvoiceStatus Status { get; private set; }
    
    public void SetStatus(InvoiceStatus status)
    {
        if(Status != status) 
        {
            Status = status;
            DomainEvents.Add(new InvoiceStatusChanged(Id, Status));
        }
    }

    public DateTime? DueDate { get; private set; }

    public void SetDueDate(DateTime dueDate)
    {
        if(DueDate != dueDate) 
        {
            DueDate = dueDate;
            DomainEvents.Add(new InvoiceDueDateChanged(Id, DueDate));
        }
    }

    public string? Reference { get; private set; }

    public void SetReference(string? reference)
    {
        if(Reference != reference) 
        {
            Reference = reference;
            DomainEvents.Add(new InvoiceReferenceChanged(Id, Reference));
        }
    }

    public decimal SubTotal { get; private set; }

    public decimal Vat { get; private set; }

    public decimal Total { get; private set; }

    public decimal? Paid { get; private set; }

    public void SetPaid(decimal amount)
    {
        if(Paid != amount) 
        {
            Paid = amount;
            DomainEvents.Add(new InvoiceAmountPaidChanged(Id, Paid));
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

    public string? Note { get; set; }

    public void SetNote(string note)
    {
        if(Note != note) 
        {
            Note = note;
            DomainEvents.Add(new InvoiceNoteChanged(Id, Note));
        }
    }

    public IReadOnlyList<InvoiceItem> Items => _items;

    public InvoiceItem AddItem(ProductType productType, string description, decimal unitPrice, string unit, double vatRate, double quantity) 
    {
        var invoiceItem = new InvoiceItem(productType, description, unitPrice, unit, vatRate, quantity);
        _items.Add(invoiceItem);

        UpdateTotals();

        return invoiceItem;
    }

    internal void UpdateTotals()
    {
        Vat = Items.Sum(i => i.Vat());
        SubTotal = Items.Sum(i => i.SubTotal());
        Total = Items.Sum(i => i.LineTotal);
    }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
