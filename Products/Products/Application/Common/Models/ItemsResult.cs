using System;

namespace YourBrand.Products.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);