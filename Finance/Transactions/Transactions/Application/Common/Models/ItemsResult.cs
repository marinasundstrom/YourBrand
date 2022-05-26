using System;

namespace YourBrand.Transactions.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);