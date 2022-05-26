namespace Transactions.Application;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);