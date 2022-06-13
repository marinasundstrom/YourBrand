namespace YourBrand.RotRutService.Application;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);