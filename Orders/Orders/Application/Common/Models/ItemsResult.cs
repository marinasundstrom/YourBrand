using System;

namespace YourBrand.Orders.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);