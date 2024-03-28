namespace YourBrand.Inventory.Contracts;

public record QuantityAvailableChanged(string Id, string WarehouseId, int Quantity);