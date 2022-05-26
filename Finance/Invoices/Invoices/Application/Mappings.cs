using Invoices.Domain.Entities;

namespace Invoices.Application;

public static class Mappings 
{
    public static InvoiceDto ToDto(this Invoice invoice) 
    {
        return new InvoiceDto(invoice.Id, invoice.Date, invoice.Type, invoice.Status, invoice.DueDate, invoice.Reference, invoice.Note, invoice.Items.Select(i => i.ToDto()),   invoice.SubTotal, invoice.Vat, invoice.Total, invoice.Paid);
    }

    public static InvoiceItemDto ToDto(this InvoiceItem item) 
    {
        return new InvoiceItemDto(item.Id, item.ProductType, item.Description, item.UnitPrice, item.Unit, item.VatRate, item.Quantity, item.LineTotal);
    }
}