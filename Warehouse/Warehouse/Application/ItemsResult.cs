namespace YourBrand.Warehouse.Application;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);