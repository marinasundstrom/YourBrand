using System;
using System.Collections.Generic;

using YourBrand.Inventory.Application.Items;
using YourBrand.Inventory.Application.Warehouses;

namespace YourBrand.Inventory.Application.Warehouses.TransferOrders;

public record TransferOrderDto(
    string Id,
    WarehouseDto SourceWarehouse,
    WarehouseDto DestinationWarehouse,
    DateTimeOffset Created,
    DateTimeOffset? CompletedAt,
    bool IsCompleted,
    IReadOnlyCollection<TransferOrderLineDto> Lines);

public record TransferOrderLineDto(
    string Id,
    ItemDto Item,
    int Quantity);

public record CreateTransferOrderDto(
    string SourceWarehouseId,
    string DestinationWarehouseId,
    IReadOnlyCollection<CreateTransferOrderLineDto> Lines);

public record CreateTransferOrderLineDto(
    string ItemId,
    int Quantity);

public record CompleteTransferOrderDto(DateTimeOffset? CompletedAt);
