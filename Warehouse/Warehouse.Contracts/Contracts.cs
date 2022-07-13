namespace YourBrand.Warehouse.Contracts;

public record InvoicesBatch(IEnumerable<Invoice> Invoices);

public record Invoice(int Id);