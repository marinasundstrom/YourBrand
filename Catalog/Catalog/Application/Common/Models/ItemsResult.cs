using System;

namespace YourBrand.Catalog.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);