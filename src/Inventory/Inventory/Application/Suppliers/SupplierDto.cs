using System;
using System.Collections.Generic;

namespace YourBrand.Inventory.Application.Suppliers;

public record SupplierDto(string Id, string Name, IReadOnlyCollection<SupplierItemDto> Items);

public record SupplierItemDto(string Id, string ItemId, string ItemName, string? SupplierItemId, decimal? UnitCost, int? LeadTimeDays);

public record SupplierOrderDto(
    string Id,
    string SupplierId,
    string SupplierName,
    string OrderNumber,
    DateTime OrderedAt,
    DateTime? ExpectedDelivery,
    IReadOnlyCollection<SupplierOrderLineDto> Lines,
    int TotalQuantityOutstanding);

public record SupplierOrderLineDto(
    string Id,
    string SupplierItemId,
    string ItemId,
    string ItemName,
    int QuantityOrdered,
    int ExpectedQuantity,
    int QuantityReceived,
    int QuantityOutstanding,
    DateTime? ExpectedOn);
