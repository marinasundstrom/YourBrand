using YourBrand.Domain;
using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Enums;

namespace YourBrand.Inventory.Domain.Events;

public record ItemCreated(string ItemId) : DomainEvent;

public record WarehouseItemCreated(string ItemId, string WarehouseId) : DomainEvent;

public record ItemAvailabilityChanged(string ItemId, ItemAvailability OldAvailability, ItemAvailability NewAvailability) : DomainEvent;

public record WarehouseItemQuantityOnHandUpdated(string ItemId, string WarehouseId, int Quantity, int OldQuantity) : DomainEvent;

public record WarehouseItemsPicked(string ItemId, string WarehouseId, int Quantity) : DomainEvent;

public record WarehouseItemsReserved(string ItemId, string WarehouseId, int Quantity) : DomainEvent;

public record WarehouseItemQuantityAvailableUpdated(string ItemId, string WarehouseId, int Quantity, int OldQuantity) : DomainEvent;

public record WarehouseItemsReservationReleased(string ItemId, string WarehouseId, int Quantity) : DomainEvent;

public record WarehouseItemsReturned(string ItemId, string WarehouseId, int Quantity) : DomainEvent;

public record WarehouseItemsUnpicked(string ItemId, string WarehouseId, int Quantity) : DomainEvent;

public record WarehouseItemsShipped(string ItemId, string WarehouseId, int Quantity) : DomainEvent;

public record WarehouseItemsTransferred(string ItemId, string SourceWarehouseId, string DestinationWarehouseId, int Quantity) : DomainEvent;