using System;

namespace YourBrand.Warehouse.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);