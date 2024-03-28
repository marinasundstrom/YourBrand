using System;

namespace YourBrand.Inventory.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);