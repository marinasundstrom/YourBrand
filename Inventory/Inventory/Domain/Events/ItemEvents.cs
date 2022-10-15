using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Events;

public record ItemCreated(string ItemId) : DomainEvent;

public record WarehouseItemCreated(string ItemId, string WarehouseId) : DomainEvent;

public record WarehouseItemQuantityOnHandUpdated(string ItemId, string WarehouseId, int Quantity, int OldQuantity) : DomainEvent;

public record WarehouseItemsPicked(string ItemId, string WarehouseId, int Quantity) : DomainEvent;

public record WarehouseItemsReserved(string ItemId, string WarehouseId, int Quantity) : DomainEvent;

public record WarehouseItemQuantityAvailableUpdated(string ItemId, string WarehouseId, int Quantity, int OldQuantity) : DomainEvent;