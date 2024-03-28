namespace YourBrand.Messenger.Application.Common.Models;

public record class Results<T>(IEnumerable<T> Items, int TotalCount);