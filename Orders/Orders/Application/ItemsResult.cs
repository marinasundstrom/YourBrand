namespace YourBrand.Orders.Application;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);