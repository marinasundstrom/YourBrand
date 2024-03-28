namespace YourBrand.Invoicing.Application;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);