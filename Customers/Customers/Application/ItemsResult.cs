namespace YourBrand.Customers.Application;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);