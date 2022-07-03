namespace YourBrand.Products.Contracts;

public record InvoicesBatch(IEnumerable<Invoice> Invoices);

public record Invoice(int Id);