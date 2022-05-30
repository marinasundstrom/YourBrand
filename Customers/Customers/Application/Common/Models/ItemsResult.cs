using System;

namespace YourBrand.Customers.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);