namespace YourBrand.Invoicing.Contracts;

public record InvoicesBatch(string OrganizationId, IEnumerable<Invoice> Invoices);

public record Invoice(string OrganizationId, string Id);

public record InvoicePaid(string OrganizationId, string Id);