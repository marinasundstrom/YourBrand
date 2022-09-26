namespace YourBrand.Invoicing.Contracts;

public record InvoicesBatch(IEnumerable<Invoice> Invoices);

public record Invoice(string Id);

public record InvoicePaid(string Id);