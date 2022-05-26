using System;

using Invoices.Domain.Enums;

namespace Invoices.Application;

public record InvoiceDto(int Id, DateTime? Date, Domain.Enums.InvoiceType Type, Domain.Enums.InvoiceStatus Status, DateTime? DueDate, string? Reference, string? Note, IEnumerable<InvoiceItemDto> Items, decimal SubTotal, decimal Vat, decimal Total, decimal? Paid);

public record InvoiceItemDto(int Id, ProductType ProductType, string Description, decimal UnitPrice, string Unit, double VatRate, double Quantity, decimal LineTotal);